namespace LedgerGateway.Dtos;

public class BalanceResponseDto
{
    public string UserEmail { get; set; }
    public string Currency { get; set; }
    public long Amount { get; set; }
}