using FinancialService.Model;
using FinancialService.Repository;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ServiceCommons;

namespace FinancialService.Application.Services;

public interface IDepositService
{
    Task<Result<DepositResult>> DepositAsync(
        Guid userId,
        long amount,
        string currency,
        string idempotencyKey,
        CancellationToken cancellationToken);
}

public class DepositService(
    ILogger<DepositService> logger,
    FinancialDbContext db) : IDepositService
{
    public async Task<Result<DepositResult>> DepositAsync(Guid userId, long amount, string currency, string idempotencyKey, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deposit {DepositAmount} for user {UserId}", amount, userId);
        
        var cashInAccount = await db.Accounts
            .FirstOrDefaultAsync(a =>
                a.Id == SystemAccounts.CashInAccountId
                && a.Currency == currency, cancellationToken: cancellationToken);

        if (cashInAccount == null)
        {
            return Result<DepositResult>.Failure("Internal CashIn account not found", ErrorType.Unexpected);
        }
        
        var userAccount = await db.Accounts
            .FirstOrDefaultAsync(a =>
                a.UserId == userId
                && a.Currency == currency, cancellationToken: cancellationToken);

        if (userAccount == null)
        {
            return Result<DepositResult>.Failure("User account not found", ErrorType.Unexpected);
        }

        var transaction = new Transaction
        {
            Id = Guid.CreateVersion7(),
            IdempotencyKey = idempotencyKey,
            Type = TransferenceType.Deposit,
            CreatedAt = DateTime.UtcNow
        };

        var debitEntry = new JournalEntry
        {
            Id = Guid.CreateVersion7(),
            TransactionId = transaction.Id,
            AccountId = SystemAccounts.CashInAccountId,
            Type = EntryType.Debit,
            Amount = amount,
            CreatedAt = DateTime.UtcNow
        };
        
        var creditEntry = new JournalEntry
        {
            Id = Guid.CreateVersion7(),
            TransactionId = transaction.Id,
            AccountId = userAccount.Id,
            Type = EntryType.Credit,
            Amount = amount,
            CreatedAt = DateTime.UtcNow
        };
        
        await db.Transactions.AddAsync(transaction, cancellationToken);
        await db.JournalEntries.AddRangeAsync([debitEntry, creditEntry], cancellationToken: cancellationToken);

        try
        {
            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Deposit {DepositAmount} for user {UserId} was successful", amount, userId);

            var depositResult = new DepositResult()
            {
                TransactionId = transaction.Id,
                IsIdempontentReplay = false
            };
            return Result<DepositResult>.Success(depositResult);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            var existing = await db.Transactions
                .AsNoTracking()
                .FirstAsync(t => t.IdempotencyKey == idempotencyKey, cancellationToken);

            logger.LogInformation("Deposit replay detected for IdempotencyKey={IdempotencyKey}", idempotencyKey);

            return Result<DepositResult>.Success(new DepositResult
            {
                TransactionId = existing.Id,
                IsIdempontentReplay = true
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Deposit {DepositAmount} for user {UserId} failed", amount, userId);
            return Result<DepositResult>.Failure("Deposit failed", ErrorType.Unexpected);
        }
    }
    
    private static bool IsUniqueViolation(DbUpdateException ex)
    {
        return ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    }
}