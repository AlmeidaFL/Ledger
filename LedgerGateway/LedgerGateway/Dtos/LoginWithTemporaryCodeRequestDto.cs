namespace LedgerGateway.Dtos;

public class LoginWithTemporaryCodeRequestDto
{
    public string Email { get; set; }

    public string Code { get; set; }
}