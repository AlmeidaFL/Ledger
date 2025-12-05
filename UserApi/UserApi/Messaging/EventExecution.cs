using Microsoft.EntityFrameworkCore;
using UserApi.Model;
using UserApi.Repository;

namespace UserApi.Messaging;

public class EventExecution<TEvent>(
    ILogger<EventExecution<TEvent>> logger,
    UserDbContext db)
{
    private readonly int _maxRetries = 3;

    public async Task ExecuteAsync(
        TEvent evt,
        Func<Task> handlerLogic,
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var attempt = 0;

        while (true)
        {
            attempt++;

            try
            {
                if (await db.ProcessedEvents
                        .AnyAsync(x => x.IdempotencyKey == eventId.ToString(), cancellationToken))
                {
                    logger.LogInformation("Duplicate event {Id} skipped", eventId);
                    return;
                }

                await using var trx = await db.Database.BeginTransactionAsync(cancellationToken);

                db.ProcessedEvents.Add(new ProcessedEvent
                {
                    IdempotencyKey = eventId.ToString(),
                    ProcessedAt = DateTime.UtcNow
                });

                await handlerLogic();

                await db.SaveChangesAsync(cancellationToken);

                await trx.CommitAsync(cancellationToken);

                logger.LogInformation(
                    "Event {EventId} processed successfully (attempt {Attempt})",
                    eventId, attempt);

                break;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (attempt >= _maxRetries)
                {
                    logger.LogError(
                        "Concurrency error for event {EventId} after {Attempt} attempts",
                        eventId, attempt);
                    throw;
                }

                logger.LogWarning(
                    "Concurrency conflict for event {EventId}, retrying... attempt {Attempt}",
                    eventId, attempt);

                await Task.Delay(100 * attempt, cancellationToken);
            }
            catch (Exception ex)
            {
                if (attempt >= _maxRetries)
                {
                    logger.LogError(ex,
                        "Unexpected error processing event {EventId} after {Attempt} attempts",
                        eventId, attempt);
                    throw;
                }

                logger.LogWarning(ex,
                    "Transient error for event {EventId}, retrying... attempt {Attempt}",
                    eventId, attempt);

                await Task.Delay(100 * attempt, cancellationToken);
            }
        }
    }
}
