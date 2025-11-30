using FinancialService.Messaging.Events;

namespace FinancialService.Application;

public interface IUserCreatedHandler
{
    Task HandleAsync(UserCreatedEvent evt, CancellationToken cancellationToken = default);
}

public class UserCreatedHandler : IUserCreatedHandler
{
    public Task HandleAsync(UserCreatedEvent evt, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}