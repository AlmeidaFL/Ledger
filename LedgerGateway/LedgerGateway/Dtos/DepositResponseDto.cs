using ServiceCommons;

namespace LedgerGateway.Dtos;

public class DepositResponseDto
{
    public string TransactionId { get; set; }
    public string Status { get; set; }
}