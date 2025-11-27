namespace UserApi.Model;

public class User
{
    public Guid Id { get; set; }

    public string Email { get; set; }
    public string FullName { get; set; }
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public byte[] RowVersion { get; set; }
    
    public Account Account { get; set; }
}