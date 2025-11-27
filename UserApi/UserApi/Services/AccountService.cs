using UserApi.Model;

namespace UserApi.Services;

public interface IAccountService
{
    Task<Account> CreateDefaultAccountForUserAsync(
        User user,
        CancellationToken cancellationToken);
    
    Task<Account?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);
}

public class AccountService : IAccountService
{
    public Task<Account> CreateDefaultAccountForUserAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Account?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}