using ServiceCommons;

namespace LedgerGateway.Dtos;

public class TransferResponseDto
{
    public string TransactionId { get; set; }
    public string Status { get; set; }
    public bool IsIdempotentReplay { get; set; }
}