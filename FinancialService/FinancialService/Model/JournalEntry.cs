namespace FinancialService.Model;

public class JournalEntry
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public Guid AccountId { get; set; }
    public EntryType Type { get; set; }
    public int Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public Transaction Transaction { get; set; }
    public Account Account { get; set; }
}