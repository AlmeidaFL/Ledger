using Microsoft.EntityFrameworkCore;
using ServiceCommons;
using ServiceCommons.Utils;
using UserApi.Converters;
using UserApi.Dtos;
using UserApi.Model;
using UserApi.Repository;
using UserApi.Services.Events;
using UserApi.Services.Exceptions;

namespace UserApi.Services;

public interface IUserService
{
    Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    Task<Result<UserResponse>> GetUserAsync(
        Guid userId,
        Guid requestingUsedId,
        CancellationToken cancellationToken = default);

    Task<Result<UserResponse>> UpdateUserAsync(
        Guid userId,
        Guid requestingUserId,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default);
    
    Task<Result> DeactivateUserAsync(
        Guid userId,
        Guid requestingUserId,
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

            var userCreatedEvent = new UserCreatedEvent(user.Id, user.Email, user.FullName);
            var message = OutboxMessage.FromEvent(userCreatedEvent);
            db.OutboxMessages.Add(message);

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

    public async Task<Result<UserResponse>> GetUserAsync(Guid userId, Guid requestingUsedId, CancellationToken cancellationToken = default)
    {
        if (!OwnershipUtils.HasOwnership(userId, requestingUsedId))
        {
            return Result<UserResponse>.Failure("User does not own this user", ErrorType.Forbidden);
        }
        
        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user == null ? 
            Result<UserResponse>.Failure("User not found", ErrorType.NotFound) 
            : Result<UserResponse>.Success(UserConverter.ToResponse(user));
    }

    public async Task<Result<UserResponse>> UpdateUserAsync(Guid userId, Guid requestingUserId, UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var hasOwnership = OwnershipUtils.HasOwnership(userId, requestingUserId);

        if (!hasOwnership)
        {
            return Result<UserResponse>.Failure("User does not own this user", ErrorType.Forbidden);
        }
        
        var user = await GetUserById(userId, cancellationToken);

        if (user == null)
        {
            return Result<UserResponse>.Failure("User not found", ErrorType.NotFound);
        }
        
        user.FullName = request.FullName;
        user.UpdatedAt = DateTime.UtcNow;
        
        await db.SaveChangesAsync(cancellationToken);
        
        return Result<UserResponse>.Success(UserConverter.ToResponse(user));
    }

    public async Task<Result> DeactivateUserAsync(Guid userId, Guid requestingUserId, CancellationToken cancellationToken = default)
    {
        var hasOwnership = OwnershipUtils.HasOwnership(userId, requestingUserId);
        if (!hasOwnership)
        {
            return Result<UserResponse>.Failure("User does not own this user", ErrorType.Forbidden);
        }
        
        var user = await GetUserById(userId, cancellationToken);
        if (user == null)
        {
            return Result<UserResponse>.Failure("User not found", ErrorType.NotFound);
        }
        
        user.IsActive = false;
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
    
    private async Task<User?> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        return user;
    }
}