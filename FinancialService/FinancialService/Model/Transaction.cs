namespace FinancialService.Model;

public class Transaction
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string IdempotencyKey { get; set; }
    public TransferenceType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // Json
    public string Metadata { get; set; } = "{}";

    public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
}