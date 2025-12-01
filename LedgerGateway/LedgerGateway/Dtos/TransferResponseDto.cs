using ServiceCommons;

namespace LedgerGateway.Dtos;

public class TransferResponseDto : IResult
{
    public string TransactionId { get; set; }
    public string Status { get; set; }
    public bool IsIdempotentReplay { get; set; }
    public Result Result { get; set; }
}