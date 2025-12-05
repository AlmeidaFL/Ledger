using Microsoft.EntityFrameworkCore;
using ServiceCommons;
using ServiceCommons.Utils;
using UserApi.Converters;
using UserApi.Dtos;
using UserApi.Messaging.Events;
using UserApi.Model;
using UserApi.Repository;
using UserApi.Services.Exceptions;

namespace UserApi.Services;

public interface IUserService
{
    Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    Task<Result<UserResponse>> GetUserAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<Result<UserResponse>> UpdateUserAsync(
        string email,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default);
    
    Task<Result> DeactivateUserAsync(
        string email,
        CancellationToken cancellationToken = default);
}

public class UserService(
    UserDbContext db,
    IAccountService accountService) : IUserService
{
    public async Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var emailExists = await db.Users
            .AsNoTracking()
            .AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (emailExists)
        {
            return Result<UserResponse>.Failure("Email already exists", ErrorType.Conflict);
        }

        var user = new User
        {
            Email = request.Email,
            FullName = request.FullName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };
        
        return await SaveUser(user, cancellationToken);
    }

    private async Task<Result<UserResponse>> SaveUser(User user, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            db.Users.Add(user);

            var result = await accountService.CreateDefaultAccountForUserAsync(user, cancellationToken);
            if (result.IsFailure)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<UserResponse>.Failure(result.Error!);
            }

            var evt = CreateUserEvent(user);
            db.OutboxMessages.Add(evt);

            await db.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);

            return Result<UserResponse>.Failure(
                "An unexpected error occurred while creating the user.",
                ErrorType.Unexpected);
        }

        return Result<UserResponse>.Success(UserConverter.ToResponse(user));
    }

    private static OutboxMessage CreateUserEvent(User user)
    {
        var userCreatedEvent = new UserCreatedEvent
        {
            Id = Guid.CreateVersion7(),
            AggregateId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
        };
        
        return OutboxMessage.FromEvent(
            message: userCreatedEvent,
            "user-api");
    }

    public async Task<Result<UserResponse>> GetUserAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await GetUserByEmail(email, cancellationToken);

        return user == null ? 
            Result<UserResponse>.Failure("User not found", ErrorType.NotFound) 
            : Result<UserResponse>.Success(UserConverter.ToResponse(user));
    }

    public async Task<Result<UserResponse>> UpdateUserAsync(string email, UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await GetUserByEmail(email, cancellationToken);

        if (user == null)
        {
            return Result<UserResponse>.Failure("User not found", ErrorType.NotFound);
        }
        
        user.FullName = request.FullName;
        user.UpdatedAt = DateTime.UtcNow;
        
        await db.SaveChangesAsync(cancellationToken);
        
        return Result<UserResponse>.Success(UserConverter.ToResponse(user));
    }

    public async Task<Result> DeactivateUserAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await GetUserByEmail(email, cancellationToken);
        if (user == null)
        {
            return Result<UserResponse>.Failure("User not found", ErrorType.NotFound);
        }
        
        user.IsDeleted = true;
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
    
    private async Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);
        return user;
    }
}