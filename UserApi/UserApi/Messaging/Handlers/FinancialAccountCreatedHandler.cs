using Microsoft.EntityFrameworkCore;
using Npgsql;
using UserApi.Messaging.Events;
using UserApi.Model;
using UserApi.Repository;

namespace UserApi.Messaging.Handlers;

public class FinancialAccountCreatedHandler(
    ILogger<FinancialAccountCreatedHandler> logger,
    UserDbContext db)
    : IMessageHandler<FinancialAccountCreatedEvent>
{
    public async Task HandleAsync(FinancialAccountCreatedEvent evt, CancellationToken cancellationToken)
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
                    "Processed FinancialAccountCreatedEvent (UserId={UserId}), attempt={Attempt}",
                    evt.UserId, attempt);
                break;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (attempt >= maxRetries)
                {
                    logger.LogError(
                        "Concurrency error processing FinancialAccountCreatedEvent for {UserId} after {Attempt} attempts",
                        evt.UserId, attempt);
                    throw;
                }

                logger.LogWarning(
                    "Concurrency conflict processing FinancialAccountCreatedEvent for {UserId}, retrying... attempt {Attempt}",
                    evt.UserId, attempt);

                await Task.Delay(100 * attempt, cancellationToken);
            }
            catch (Exception ex)
            {
                if (attempt >= maxRetries)
                {
                    logger.LogError(ex,
                        "Unexpected error processing FinancialAccountCreatedEvent for {UserId} after {Attempt} attempts",
                        evt.UserId, attempt);
                    throw;
                }

                logger.LogWarning(ex,
                    "Unexpected conflict processing FinancialAccountCreatedEvent for {UserId}, retrying... attempt {Attempt}",
                    evt.UserId, attempt);

                await Task.Delay(100 * attempt, cancellationToken);
            }
        }
    }

    private async Task Handle(FinancialAccountCreatedEvent evt, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        db.ProcessedEvents.Add(new ProcessedEvent
        {
            IdempotencyKey = evt.Id.ToString(),
            ProcessedAt = DateTime.UtcNow
        });

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

        var user = await db.Users
            .FirstOrDefaultAsync(x => x.Id == evt.UserId, cancellationToken);

        if (user != null &&
            state is { UserCreatedReceived: true, FinancialAccountCreatedReceived: true })
        {
            user.IsActive = true;
        }

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}