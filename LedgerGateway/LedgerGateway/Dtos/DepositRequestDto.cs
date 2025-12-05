namespace LedgerGateway.Dtos;

public class DepositRequestDto
{
    public string UserEmail { get; set; }
    public long Amount { get; set; }
    public string Currency { get; set; }
    public string IdempotencyKey { get; set; }
}