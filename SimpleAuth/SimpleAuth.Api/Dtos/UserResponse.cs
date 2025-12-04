using System;

namespace SimpleAuth.Api.Dtos;

public class UserResponse
{
    public Guid Id { get; set; }
    public string? GoogleId { get; set; }
    public bool IsGoogleUser => GoogleId != null;
    public string Email { get; set; } = string.Empty;
    public DateTime TemporaryPasswordExpiresAt { get; set; }
}