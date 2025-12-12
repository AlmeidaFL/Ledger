using LedgerGateway.Dtos;
using LedgerGateway.RestClients.SimpleAuth;

namespace LedgerGateway.Converters;

public static class RegisterRequestConverter
{
    public static RegisterRequest ToRegisterRequest(this RegisterRequestDto dto)
    {
        return new RegisterRequest
        {
            Email = dto.Email,
            Password = dto.Password,
            Fullname = dto.Fullname,
        };
    }
}