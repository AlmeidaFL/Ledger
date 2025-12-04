using LedgerGateway.Dtos;
using LedgerGateway.RestClients.SimpleAuth;

namespace LedgerGateway.Integration;

public static class GoogleLoginRequestConverter
{
    public static GoogleLoginRequest Convert(this GoogleLoginRequestDto dto)
    {
        return new GoogleLoginRequest
        {
            IdToken = dto.IdToken,
        };
    }
}