using Microsoft.EntityFrameworkCore;
using ServiceCommons;
using UserApi.Model;
using UserApi.Repository;

namespace UserApi.Services;

public interface IAccountService
{
    Task<Result<Account>> CreateDefaultAccountForUserAsync(
        User user,
        CancellationToken cancellationToken);
    
    Task<Account?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);
}

public class AccountService(UserDbContext db) : IAccountService
{
    public async Task<Result<Account>> CreateDefaultAccountForUserAsync(User user, CancellationToken cancellationToken)
    {
        var existingAccount = await GetByUserIdAsync(user.Id, cancellationToken);

        if (existingAccount is not null)
        {
            return Result<Account>.Failure("Account already exists");
        }

        var account = new Account()
        {
            UserId = user.Id,
            AccountNumber = GenerateAccountNumber(),
            AccountType = "checking",
            Status = AccountStatus.Pending,
            CreatedAt = DateTime.UtcNow,
        };

        try
        {
            db.Accounts.Add(account);
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return Result<Account>.Failure(ex.Message);
        }

        return Result<Account>.Success(account);
    }

    private static string GenerateAccountNumber()
    {
        return Random.Shared.Next(10000000, 99999999).ToString();
    }

    public Task<Account?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return db.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
    }
}