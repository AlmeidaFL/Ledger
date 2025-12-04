using System;

namespace SimpleAuth.Api.Model;

public class LoginAttempt
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string IpAddress { get; set; }
    public DateTime AttemptAt { get; set; }
    public bool Success { get; set; }
}