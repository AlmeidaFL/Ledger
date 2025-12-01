using FinancialService.Repository;
using Microsoft.EntityFrameworkCore;

namespace FinancialService.Application.Services;

public interface IAccountLockService
{
    Task LockAccountsAsync(Guid accountA, Guid accountB, CancellationToken cancellationToken = default);
}

public class AccountLockService(FinancialDbContext db) : IAccountLockService
{
    public async Task LockAccountsAsync(Guid accountA, Guid accountB, CancellationToken ct)
    {
        var (first, second) = Order(accountA, accountB);

        await db.Database.ExecuteSqlRawAsync(
            """
            SELECT 42 FROM "AccountLocks" 
                          WHERE "AccountId" = {0} 
                          FOR UPDATE
            """,
            first);

        await db.Database.ExecuteSqlRawAsync(
            """
            SELECT 42 FROM "AccountLocks" 
                          WHERE "AccountId" = {0} 
                          FOR UPDATE
            """,
            second);
    }

    private static (Guid first, Guid second) Order(Guid a, Guid b)
    {
        return a.CompareTo(b) < 0 ? (a, b) : (b, a);
    }
}