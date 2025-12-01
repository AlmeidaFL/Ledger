namespace LedgerGateway.Dtos;

public class TransferRequestDto
{
    public string FromAccountId { get; set; }
    public string ToAccountId { get; set; }
    public long Amount { get; set; }
    public string Currency { get; set; }
    public string IdempotencyKey { get; set; }
    public string? Metadata { get; set; }
}