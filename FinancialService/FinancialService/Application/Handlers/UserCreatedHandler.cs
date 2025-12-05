using FinancialService.Messaging;
using FinancialService.Messaging.Events;
using FinancialService.Model;
using FinancialService.Repository;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ServiceCommons;

namespace FinancialService.Application;

public interface IUserCreatedHandler
{
    Task HandleAsync(UserCreatedEvent evt, CancellationToken cancellationToken = default);
}

public class UserCreatedHandler(
    ILogger<UserCreatedHandler> logger,
    FinancialDbContext dbContext): IUserCreatedHandler
{
    public async Task HandleAsync(UserCreatedEvent evt, CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            Id = evt.AggregateId,
            Email = evt.Email,
            Name = evt.Name,
            CreatedAt = DateTime.UtcNow
        };

        var account = new Account
        {
            Id = Guid.CreateVersion7(),
            CreatedAt = DateTime.UtcNow,
            UserId = evt.AggregateId,
            Currency = "BRL",
        };
        
        dbContext.Users.Add(user);
        dbContext.Accounts.Add(account);
        var message = CreateEvent(account, user);
        dbContext.OutboxMessages.Add(OutboxMessage.FromEvent(message, "financial-service", TopicNames.AccountBalanceCreatedEvent));

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            logger.LogError(
                    ex, 
                    "User {UserId} has already been created with Account " +
                    "{AccountId} and Currency {AccountCurrency}", evt.Id, account.Id, account.Currency);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception handling user {UserId}", evt.Id);
        }
    }

    private static FinancialAccountCreatedEvent CreateEvent(Account account, User user)
    {
        var message = new FinancialAccountCreatedEvent()
        {
            Id = Guid.CreateVersion7(),
            AggregateId = account.Id,
            UserId = user.Id,
        };
        return message;
    }

    private static bool IsUniqueViolation(DbUpdateException ex)
    {
        return ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    }
}