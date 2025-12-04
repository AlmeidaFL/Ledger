using LedgerGateway.Dtos;
using LedgerGateway.RestClients.SimpleAuth;

namespace LedgerGateway.Integration;

public static class LoginWithTemporaryCodeRequestConverter
{
    public static LoginWithTemporaryCodeRequest Convert(
        this LoginWithTemporaryCodeRequestDto dto)
    {
        return new LoginWithTemporaryCodeRequest()
        {
            Code = dto.Code,
            Email = dto.Email,
        };
    }
}