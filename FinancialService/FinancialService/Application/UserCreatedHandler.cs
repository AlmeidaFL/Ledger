using FinancialService.Messaging.Events;
using FinancialService.Model;
using FinancialService.Repository;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
            Id = evt.Id,
            Email = evt.Email,
            Name = evt.FullName,
            CreatedAt = DateTime.UtcNow
        };

        var account = new Account
        {
            Id = Guid.CreateVersion7(),
            CreatedAt = DateTime.UtcNow,
            UserId = evt.Id,
            Currency = "BRL",
        };
        
        dbContext.Users.Add(user);
        dbContext.Accounts.Add(account);
        var message = CreateEvent(account, user);
        dbContext.OutboxMessages.Add(OutboxMessage.FromEvent(message, OutboxMessage.FinancialAccountCreatedTopic));

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
            Id = Guid.NewGuid().ToString(),
            AccountId = account.Id,
            UserId = user.Id,
        };
        return message;
    }

    private static bool IsUniqueViolation(DbUpdateException ex)
    {
        return ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    }
}