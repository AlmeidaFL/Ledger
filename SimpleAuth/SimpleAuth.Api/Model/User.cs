namespace SimpleAuth.Api.Data;

public class User
{
    public byte[] RowVersion { get; set; }
    public Guid Id { get; set; }
    public string? GoogleId { get; set; }
    public bool IsGoogleUser => GoogleId != null;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? TemporaryPasswordHash { get; set; }
    public DateTimeOffset TemporaryPasswordExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}