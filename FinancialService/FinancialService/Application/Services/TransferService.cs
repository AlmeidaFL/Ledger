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
        Guid fromAccountId,
        Guid toAccountId,
        long amount,
        string currency,
        string idempotencyKey,
        string? metadata,
        CancellationToken cancellationToken = default);
}

public class TransferService(
    ILogger<TransferService> logger,
    IAccountLockService accountLockService,
    FinancialDbContext db) : ITransferService
{
    public async Task<Result<TransferResult>> TransferAsync(
        Guid fromAccountId, 
        Guid toAccountId, 
        long amount, 
        string currency, 
        string idempotencyKey,
        string? metadata, 
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Starting transfer {Amount} {Currency} from {From} to {To}",
            amount, currency, fromAccountId, toAccountId);

        if (amount <= 0)
        {
            return Result<TransferResult>.Failure("Transfer amount must be greater than 0");
        }
        
        var fromAccount = await db.Accounts
            .FirstOrDefaultAsync(a => a.Id == fromAccountId && a.Currency == currency, cancellationToken);
        if (fromAccount == null)
        {
            return Result<TransferResult>.Failure("Source account not found", ErrorType.Unexpected);
        }

        var toAccount = await db.Accounts
            .FirstOrDefaultAsync(a => a.Id == toAccountId && a.Currency == currency, cancellationToken);
        if (toAccount == null)
        {
            return Result<TransferResult>.Failure("Destination account not found", ErrorType.Unexpected);
        }
        
        return await ProcessTransferAsync(fromAccountId, toAccountId, amount, currency, idempotencyKey, fromAccount, cancellationToken);
    }

    private async Task<Result<TransferResult>> ProcessTransferAsync(
        Guid fromAccountId,
        Guid toAccountId,
        long amount,
        string currency,
        string idempotencyKey,
        Account fromAccount,
        CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        var existing = await db.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.IdempotencyKey == idempotencyKey, cancellationToken);

        if (existing != null)
        {
            logger.LogInformation("Transfer replay detected for key {IdempotencyKey}", idempotencyKey);

            await tx.RollbackAsync(cancellationToken);

            return Result<TransferResult>.Success(new TransferResult
            {
                TransactionId = existing.Id,
                IsIdempotentReplay = true
            });
        }

        await accountLockService.LockAccountsAsync(fromAccountId, toAccountId, cancellationToken);

        var balance = await CalculateBalanceAsync(fromAccountId, cancellationToken);
        
        if (balance < amount)
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
                amount, currency, amount, fromAccount);

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