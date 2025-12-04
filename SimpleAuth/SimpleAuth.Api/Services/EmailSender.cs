using System;
using System.Threading.Tasks;

namespace SimpleAuth.Api.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);    
}

public class FakeEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        Console.WriteLine(message);
        return Task.CompletedTask;
    }
}