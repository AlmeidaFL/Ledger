using ServiceCommons;
using UserApi.Converters;
using UserApi.Dtos;

namespace UserApi.Services;

public interface IUserService
{
    Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    Task<UserResponse> GetUserAsync(
        Guid userId,
        Guid requestingUsedId,
        CancellationToken cancellationToken = default);

    Task<Result<UserResponse>> UpdateUserAsync(
        Guid userId,
        Guid requestingUsedId,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default);
    
    Task DeactivateUserAsync(
        Guid userId,
        Guid requestingUserId,
        CancellationToken cancellationToken = default);
}

public class UserService : IUserService
{
    public Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserResponse> GetUserAsync(Guid userId, Guid requestingUsedId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserResponse>> UpdateUserAsync(Guid userId, Guid requestingUsedId, UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeactivateUserAsync(Guid userId, Guid requestingUserId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}