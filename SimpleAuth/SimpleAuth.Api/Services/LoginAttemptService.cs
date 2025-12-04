using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Api.Model;
using SimpleAuth.Api.Repository;

namespace SimpleAuth.Api.Services;

public interface ILoginAttemptService
{
    Task Register(string email, string ipAddress, bool success);
    Task<int> CountRecentFailuresByEmail(string email, TimeSpan window);
    Task<int> CountRecentFailuresByIp(string ipAddress, TimeSpan window);
    Task<bool> IsBlocked(string email, string ip);
}

public class LoginAttemptService(AuthDbContext dbContext) : ILoginAttemptService
{
    public async Task Register(string email, string ipAddress, bool success)
    {
        var attempt = new LoginAttempt
        {
            Id = Guid.NewGuid(),
            Email = email,
            IpAddress = ipAddress,
            Success = success,
            AttemptAt = DateTime.UtcNow
        };
        
        await dbContext.LoginAttempts.AddAsync(attempt);
        await dbContext.SaveChangesAsync();
    }

    public async Task<int> CountRecentFailuresByEmail(string email, TimeSpan window)
    {
        var since = DateTime.UtcNow.Subtract(window);

        return await dbContext.LoginAttempts
            .Where(x => x.Email == email && !x.Success && x.AttemptAt >= since)
            .CountAsync();
    }

    public async Task<int> CountRecentFailuresByIp(string ipAddress, TimeSpan window)
    {
        var since = DateTime.UtcNow.Subtract(window);

        return await dbContext.LoginAttempts
            .Where(x => x.IpAddress == ipAddress && !x.Success && x.AttemptAt >= since)
            .CountAsync();
    }

    public async Task<bool> IsBlocked(string email, string ip)
    {
        var now = DateTimeOffset.UtcNow;
        
        var failures15M = await CountRecentFailuresByEmail(email, TimeSpan.FromMinutes(15));
        if (failures15M >= 5)
        {
            return true;
        }
        
        var failures30M = await CountRecentFailuresByEmail(email, TimeSpan.FromMinutes(30));
        if (failures30M >= 10)
        {
            return true;
        }
        
        var failuresIp5M = await CountRecentFailuresByIp(ip, TimeSpan.FromMinutes(5));
        return failuresIp5M >= 20;
    }
}