using FinancialService.Application.Utils;
using FinancialService.Dtos;
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
    FinancialDbContext db,
    IAccountLockService lockService) : IDepositService
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

        return await ProcessDepositAsync(userId, amount, idempotencyKey, userAccount, cancellationToken);
    }

    private async Task<Result<DepositResult>> ProcessDepositAsync(Guid userId, long amount, string idempotencyKey, Account userAccount,
        CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        // We just lock the user account because the system account will not have GET balances associated with it
        await lockService.LockAccountAsync(userId, cancellationToken: cancellationToken);
        
        var transaction = new Transaction
        {
            IdempotencyKey = idempotencyKey,
            Type = TransferenceType.Deposit,
        };

        var debitEntry = new JournalEntry
        {
            TransactionId = transaction.Id,
            AccountId = SystemAccounts.CashInAccountId,
            Type = EntryType.Debit,
            Amount = amount,
        };
        
        var creditEntry = new JournalEntry
        {
            TransactionId = transaction.Id,
            AccountId = userAccount.Id,
            Type = EntryType.Credit,
            Amount = amount,
        };
        
        await db.Transactions.AddAsync(transaction, cancellationToken);
        await db.JournalEntries.AddRangeAsync([debitEntry, creditEntry], cancellationToken: cancellationToken);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            logger.LogInformation("Deposit {DepositAmount} for user {UserId} was successful", amount, userId);

            var depositResult = new DepositResult()
            {
                TransactionId = transaction.Id,
                IsIdempontentReplay = false
            };
            return Result<DepositResult>.Success(depositResult);
        }
        catch (DbUpdateException ex) when (ExceptionUtils.IsUniqueViolation(ex))
        {
            await tx.RollbackAsync(cancellationToken);

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
            await tx.RollbackAsync(cancellationToken);

            logger.LogError(ex, "Deposit {DepositAmount} for user {UserId} failed", amount, userId);
            return Result<DepositResult>.Failure("Deposit failed", ErrorType.Unexpected);
        }
    }
}