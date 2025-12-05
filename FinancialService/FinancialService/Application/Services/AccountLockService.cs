using FinancialService.Repository;
using Microsoft.EntityFrameworkCore;

namespace FinancialService.Application.Services;

public interface IAccountLockService
{
    Task LockAccountsAsync(Guid accountA, Guid accountB, CancellationToken cancellationToken = default);
    Task LockAccountAsync(Guid userId, CancellationToken cancellationToken = default);
}

public class AccountLockService(FinancialDbContext db) : IAccountLockService
{
    public async Task LockAccountsAsync(Guid accountA, Guid accountB, CancellationToken ct)
    {
        var (first, second) = Order(accountA, accountB);

        await db.Database.ExecuteSqlRawAsync(
            """
            SELECT 42 FROM "AccountLocks" 
                          WHERE "Id" = {0} 
                          FOR UPDATE
            """,
            first);

        await db.Database.ExecuteSqlRawAsync(
            """
            SELECT 42 FROM "AccountLocks" 
                          WHERE "Id" = {0} 
                          FOR UPDATE
            """,
            second);
    }

    public async Task LockAccountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await db.Database.ExecuteSqlRawAsync(
            """
            SELECT 42 FROM "AccountLocks" 
                          WHERE "Id" = {0} 
                          FOR UPDATE
            """,
            userId);
    }

    private static (Guid first, Guid second) Order(Guid a, Guid b)
    {
        return a.CompareTo(b) < 0 ? (a, b) : (b, a);
    }
}