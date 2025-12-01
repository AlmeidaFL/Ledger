namespace FinancialService.Dtos;

public class TransferResult
{
    public Guid TransactionId { get; set; }
    public bool IsIdempotentReplay { get; set; }
}