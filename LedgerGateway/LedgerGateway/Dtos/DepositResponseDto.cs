using ServiceCommons;

namespace LedgerGateway.Dtos;

public class DepositResponseDto : IResult
{
    public string TransactionId { get; set; }
    public string Status { get; set; }
    public Result Result { get; set; }
}