using SimpleAuth.Api.Services;

namespace SimpleAuth.Api.Dtos;

public class LoginRequest
{
    public required string Email { get; init; }

    public required string Password { get; init; }
    
    public required UserAgentInfo UserAgentInfo { get; init; }
}