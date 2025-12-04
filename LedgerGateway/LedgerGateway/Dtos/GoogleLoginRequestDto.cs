using LedgerGateway.RestClients.SimpleAuth;

namespace LedgerGateway.Dtos;

public class GoogleLoginRequestDto
{
    public string IdToken { get; set; }
}