namespace UserApi.Model;

public class Account
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public string AccountNumber { get; set; }
    public string AccountType { get; set; }
    public string Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public byte[] RowVersion { get; set; }
}