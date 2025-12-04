using System;

namespace SimpleAuth.Api.Model;

public class User
{
    public uint RowVersion { get; set; }
    public Guid Id { get; set; }
    public string? GoogleId { get; set; }
    public bool IsGoogleUser => GoogleId != null;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? TemporaryPasswordHash { get; set; }
    public DateTime TemporaryPasswordExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}