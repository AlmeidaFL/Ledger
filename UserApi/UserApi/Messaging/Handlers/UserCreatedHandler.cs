using UserApi.Messaging.Events;
using UserApi.Repository;
using UserApi.Services;

namespace UserApi.Messaging.Handlers;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Messaging;
using Model;

public class UserCreatedHandler(
    EventExecution<UserCreatedEvent> executor,
    UserDbContext db,
    ILogger<UserCreatedHandler> logger,
    IAccountService accounts)
    : IMessageHandler<UserCreatedEvent>
{
    private readonly ILogger<UserCreatedHandler> _logger = logger;

    public Task HandleAsync(UserCreatedEvent evt, CancellationToken cancellationToken)
    {
        return executor.ExecuteAsync(
            evt,
            () => HandleInternalAsync(evt, cancellationToken),
            eventId: evt.Id,
            cancellationToken);
    }

    private async Task HandleInternalAsync(UserCreatedEvent evt, CancellationToken cancellationToken)
    {
        var state = await db.ProvisioningStates
            .FirstOrDefaultAsync(x => x.UserId == evt.AggregateId, cancellationToken);

        if (state == null)
        {
            state = new UserProvisioningState
            {
                UserId = evt.AggregateId,
                UserCreatedReceived = true,
            };
            db.ProvisioningStates.Add(state);
        }
        else
        {
            state.UserCreatedReceived = true;
        }

        var user = await db.Users
            .FirstOrDefaultAsync(x => x.Id == evt.AggregateId, cancellationToken);

        if (user == null)
        {
            user = new User
            {
                Id = evt.AggregateId,
                Email = evt.Email,
                FullName = evt.Name,
                CreatedAt = evt.CreatedAt,
                IsActive = false
            };

            db.Users.Add(user);
            await accounts.CreateDefaultAccountForUserAsync(user, cancellationToken);
        }

        if (state.UserCreatedReceived && state.FinancialAccountCreatedReceived)
            user.IsActive = true;
    }
}
