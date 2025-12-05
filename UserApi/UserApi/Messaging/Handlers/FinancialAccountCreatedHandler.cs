using Microsoft.EntityFrameworkCore;
using UserApi.Messaging.Events;
using UserApi.Model;
using UserApi.Repository;

namespace UserApi.Messaging.Handlers;

public class FinancialAccountCreatedHandler(
    EventExecution<FinancialAccountCreatedEvent> pipeline,
    UserDbContext db)
    : IMessageHandler<FinancialAccountCreatedEvent>
{
    public Task HandleAsync(FinancialAccountCreatedEvent evt, CancellationToken cancellationToken)
    {
        return pipeline.ExecuteAsync(
            evt,
            () => HandleInternalAsync(evt, cancellationToken),
            eventId: evt.Id,
            cancellationToken);
    }

    private async Task HandleInternalAsync(FinancialAccountCreatedEvent evt, CancellationToken cancellationToken)
    {
        var state = await db.ProvisioningStates
            .FirstOrDefaultAsync(x => x.UserId == evt.UserId, cancellationToken);

        if (state == null)
        {
            state = new UserProvisioningState
            {
                UserId = evt.UserId,
                FinancialAccountCreatedReceived = true
            };
            db.ProvisioningStates.Add(state);
        }
        else
        {
            state.FinancialAccountCreatedReceived = true;
        }

        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == evt.UserId, cancellationToken);

        if (user != null &&
            state.UserCreatedReceived &&
            state.FinancialAccountCreatedReceived)
        {
            user.IsActive = true;
        }
    }
}