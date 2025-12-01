namespace FinancialService.Dtos;

public class DepositResult
{
    public Guid TransactionId { get; set; }
    public bool IsIdempontentReplay { get; set; }
}