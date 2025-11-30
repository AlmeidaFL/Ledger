namespace FinancialService.Model;

public class Transaction
{
    public Guid Id { get; set; }
    public string IdempotencyKey { get; set; }
    public TransferenceType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    // Json
    public string Metadata { get; set; } = "{}";

    public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
}