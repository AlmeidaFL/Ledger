using Microsoft.EntityFrameworkCore;
using Npgsql;
using UserApi.Messaging.Events;
using UserApi.Model;
using UserApi.Repository;

namespace UserApi.Application.Handlers;

public interface IFinancialAccountCreatedHandler
{
    Task HandleAsync(FinancialAccountCreatedEvent evt, CancellationToken cancellationToken);
}

public class FinancialAccountCreatedHandler(
    UserDbContext dbContext,
    ILogger<FinancialAccountCreatedEvent> logger) : IFinancialAccountCreatedHandler
{
    public async Task HandleAsync(FinancialAccountCreatedEvent evt, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing event {EventId}", evt.Id);
            
        var user = await dbContext.Users.Where(u => u.Id == evt.UserId).SingleOrDefaultAsync(cancellationToken);
        if (user == null)
        {
            logger.LogWarning("User {UserId} not found", evt.UserId);
        }
        
        var processedEvent = new ProcessedEvent
        {
            IdempotencyKey = evt.Id,
            ProcessedAt = DateTime.UtcNow
        };
        
        dbContext.ProcessedEvents.Add(processedEvent);
        user!.IsActive = true;

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            logger.LogError(
                ex, 
                "The event {EventId} has been already processed." +
                "User {UserId} should be already active", evt.Id, evt.UserId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception handling event {UserId}", evt.Id);
        }
    }
    
    private static bool IsUniqueViolation(DbUpdateException ex)
    {
        return ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    }
}