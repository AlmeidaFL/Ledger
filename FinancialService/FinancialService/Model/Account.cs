namespace FinancialService.Model;

public class Account
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Currency { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public User User { get; set; }
    public ICollection<JournalEntry> Entries { get; set; }
}