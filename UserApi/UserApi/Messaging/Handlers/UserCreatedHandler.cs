using UserApi.Messaging.Events;
using UserApi.Repository;
using UserApi.Services;

namespace UserApi.Messaging.Handlers;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Messaging;
using Model;

public class UserCreatedHandler(ILogger<UserCreatedHandler> logger, UserDbContext db, IAccountService accountService)
    : IMessageHandler<UserCreatedEvent>
{
    public async Task HandleAsync(UserCreatedEvent evt, CancellationToken cancellationToken)
    {
        const int maxRetries = 3;
        var attempt = 0;

        while (true)
        {
            attempt++;

            try
            {
                if (await db.ProcessedEvents.AnyAsync(x => x.IdempotencyKey == evt.Id.ToString(), cancellationToken))
                {
                    logger.LogInformation("Duplicate event {Id} skipped", evt.Id);
                    break;
                }

                await Handle(evt, cancellationToken);

                logger.LogInformation(
                    "Processed UserCreatedEvent (UserId={UserId}), attempt={Attempt}",
                    evt.AggregateId, attempt);

                break; 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (attempt >= maxRetries)
                {
                    logger.LogError(
                        "Concurrency error processing UserCreatedEvent for {UserId} after {Attempt} attempts",
                        evt.AggregateId, attempt);
                    throw;
                }

                logger.LogWarning(
                    "Concurrency conflict processing UserCreatedEvent for {UserId}, retrying... attempt {Attempt}",
                    evt.AggregateId, attempt);

                await Task.Delay(100 * attempt, cancellationToken);
            }
            catch (Exception ex)
            {
                if (attempt >= maxRetries)
                {
                    logger.LogError(ex,
                        "Unexpected error processing UserCreatedEvent for {UserId} after {Attempt} attempts",
                        evt.AggregateId, attempt);
                    throw;
                }

                logger.LogWarning(ex,
                    "Unexpected conflict processing UserCreatedEvent for {UserId}, retrying... attempt {Attempt}",
                    evt.AggregateId, attempt);

                await Task.Delay(100 * attempt, cancellationToken);
            }
        }
    }

    private async Task Handle(UserCreatedEvent evt, CancellationToken cancellationToken)
    {
        await using var trx = await db.Database.BeginTransactionAsync(cancellationToken);

        db.ProcessedEvents.Add(new ProcessedEvent
        {
            IdempotencyKey = evt.Id.ToString(),
            ProcessedAt = DateTime.UtcNow
        });

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

            await db.Users.AddAsync(user, cancellationToken);
            await accountService.CreateDefaultAccountForUserAsync(user, cancellationToken);
        }

        if (state is { UserCreatedReceived: true, FinancialAccountCreatedReceived: true })
            user.IsActive = true;

        await db.SaveChangesAsync(cancellationToken);

        await trx.CommitAsync(cancellationToken);
    }
}
