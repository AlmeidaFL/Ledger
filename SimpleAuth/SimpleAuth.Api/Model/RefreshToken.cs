using System;

namespace SimpleAuth.Api.Model;

public class RefreshToken
{
    public uint RowVersion { get; set; }
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool Revoked { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;
    
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    
    public string BrowserFamily { get; set; } = string.Empty;
    public string BrowserVersion { get; set; } = string.Empty;
    public string DeviceFamily { get; set; } = string.Empty;
    public string DeviceBrand { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ClientType { get; set; } = string.Empty;
}