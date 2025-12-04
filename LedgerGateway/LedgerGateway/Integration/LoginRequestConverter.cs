using LedgerGateway.Dtos;
using LedgerGateway.RestClients.SimpleAuth;

namespace LedgerGateway.Integration;

public static class LoginRequestConverter
{
    public static LoginRequest Convert(this LoginRequestDto loginRequest)
    {
        return new LoginRequest
        {
            Email = loginRequest.Email,
            Password = loginRequest.Password,
        };
    }
}