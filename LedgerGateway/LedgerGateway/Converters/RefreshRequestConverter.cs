using LedgerGateway.Dtos;
using LedgerGateway.RestClients.SimpleAuth;

namespace LedgerGateway.Converters;

public static class RefreshRequestConverter
{
    public static RefreshRequest Convert(this RefreshRequestDto dto)
    {
        return new RefreshRequest
        {
            RefreshToken = dto.Token
        };
    }
}