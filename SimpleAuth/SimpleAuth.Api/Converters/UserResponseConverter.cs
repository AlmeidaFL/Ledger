using SimpleAuth.Api.Dtos;
using SimpleAuth.Api.Model;

namespace SimpleAuth.Api.Converters;

public static class UserResponseConverter
{
    public static UserResponse Convert(User user)
    {
        return new UserResponse()
        {
            Id = user.Id,
            Email = user.Email,
            TemporaryPasswordExpiresAt = user.TemporaryPasswordExpiresAt,
            GoogleId = user.GoogleId,
        };
    }
}