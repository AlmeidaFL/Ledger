namespace LedgerGateway.Dtos;

public class DepositRequestDto
{
    public string UserId { get; set; }
    public long Amount { get; set; }
    public string Currency { get; set; }
    public string IdempotencyKey { get; set; }
}