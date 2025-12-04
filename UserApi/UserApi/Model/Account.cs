namespace UserApi.Model;

public class Account
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public string AccountNumber { get; set; }
    public string AccountType { get; set; }
    public AccountStatus Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public uint RowVersion { get; set; }
}

public enum AccountStatus
{
    Pending,
    Active,
    Disabled
}