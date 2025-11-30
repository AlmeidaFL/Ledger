namespace FinancialService.Model;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}