using UserApi.Dtos;
using UserApi.Model;

namespace UserApi.Converters;

public class UserConverter
{
    public static UserResponse ToResponse(User user)
        => new()
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
}