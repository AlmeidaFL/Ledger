using FinancialService.Application.Utils;
using FinancialService.Dtos;
using FinancialService.Model;
using FinancialService.Repository;
using Microsoft.EntityFrameworkCore;
using ServiceCommons;

namespace FinancialService.Application.Services;

public interface ITransferService
{
    Task<Result<TransferResult>> TransferAsync(
        string fromAccountEmail,
        string toAccountEmail,
        long amount,
        string currency,
        string idempotencyKey,
        string? metadata,
        CancellationToken cancellationToken = default);
}

public class TransferService(
    ILogger<TransferService> logger,
    IAccountLockService accountLockService,
    IBalanceService balanceService,
    FinancialDbContext db) : ITransferService
{
    public async Task<Result<TransferResult>> TransferAsync(
        string fromAccountEmail, 
        string toAccountEmail, 
        long amount, 
        string currency, 
        string idempotencyKey,
        string? metadata, 
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Starting transfer {Amount} {Currency} from {From} to {To}",
            amount, currency, fromAccountEmail, toAccountEmail);

        if (amount <= 0)
        {
            return Result<TransferResult>.Failure("Transfer amount must be greater than 0");
        }
        
        var fromAccount = await db.Accounts
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.User.Email == fromAccountEmail && a.Currency == currency, cancellationToken);
        if (fromAccount == null)
        {
            return Result<TransferResult>.Failure("Source account not found", ErrorType.Unexpected);
        }

        var toAccount = await db.Accounts
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.User.Email == toAccountEmail && a.Currency == currency, cancellationToken);
        if (toAccount == null)
        {
            return Result<TransferResult>.Failure("Destination account not found", ErrorType.Unexpected);
        }
        
        var existing = await db.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.IdempotencyKey == idempotencyKey, cancellationToken);

        if (existing == null)
            return await ProcessTransferAsync(
                fromAccount.Id, 
                toAccount.Id, 
                fromAccountEmail,
                amount, 
                currency, 
                idempotencyKey,
                cancellationToken);
       
        logger.LogInformation("Transfer replay detected for key {IdempotencyKey}", idempotencyKey);

        return Result<TransferResult>.Success(new TransferResult
        {
            TransactionId = existing.Id,
            IsIdempotentReplay = true
        });
    }

    private async Task<Result<TransferResult>> ProcessTransferAsync(
        Guid fromAccountId,
        Guid toAccountId,
        string fromUserEmail,
        long amount,
        string currency,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        await accountLockService.LockAccountsAsync(fromAccountId, toAccountId, cancellationToken);

        var balance = await balanceService.GetBalance(fromUserEmail, cancellationToken);

        if (balance.IsFailure)
        {
            return Result<TransferResult>.Failure(balance);
        }
        
        if (balance.Value!.BalanceInCents < amount)
        {
            await tx.RollbackAsync(cancellationToken);

            return Result<TransferResult>.Failure(
                "Insufficient funds");
        }

        var transaction = new Transaction
        {
            IdempotencyKey = idempotencyKey,
            Type = TransferenceType.Transfer,
        };

        var debitEntry = new JournalEntry
        {
            TransactionId = transaction.Id,
            AccountId = fromAccountId,
            Type = EntryType.Debit,
            Amount = amount,
        };
        
        var creditEntry = new JournalEntry
        {
            TransactionId = transaction.Id,
            AccountId = toAccountId,
            Type = EntryType.Credit,
            Amount = amount,
        };

        try
        {
            await db.Transactions.AddAsync(transaction, cancellationToken);
            await db.JournalEntries.AddRangeAsync([debitEntry, creditEntry], cancellationToken);
            
            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            
            logger.LogInformation(
                "Transfer completed: {TransferCurrency}{TransferAmount} {TransferFrom} -> {TransferTo}",
                currency, amount, fromAccountId, toAccountId);

            return Result<TransferResult>.Success(new TransferResult
            {
                TransactionId = transaction.Id,
                IsIdempotentReplay = false,
            });
        }
        catch (DbUpdateException ex) when (ExceptionUtils.IsUniqueViolation(ex))
        {
            var committed = await db.Transactions
                .AsNoTracking()
                .FirstAsync(t => t.IdempotencyKey == idempotencyKey, cancellationToken);

            await tx.RollbackAsync(cancellationToken);

            return Result<TransferResult>.Success(new TransferResult
            {
                TransactionId = committed.Id,
                IsIdempotentReplay = true
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Transfer failed internal error");
            await tx.RollbackAsync(cancellationToken);

            return Result<TransferResult>.Failure("Transfer failed", ErrorType.Unexpected);
        }
    }

    private async Task<long> CalculateBalanceAsync(Guid accountId, CancellationToken ct)
    {
        return await db.JournalEntries
            .Where(e => e.AccountId == accountId)
            .SumAsync(
                e => e.Type == EntryType.Credit ? e.Amount : -e.Amount,
                ct
            );
    }
}