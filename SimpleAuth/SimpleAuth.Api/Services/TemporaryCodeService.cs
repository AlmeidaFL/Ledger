using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServiceCommons;
using SimpleAuth.Api.Dtos;
using SimpleAuth.Api.Repository;

namespace SimpleAuth.Api.Services;

public interface ITemporaryCodeService
{
    Task GenerateAndSendCode(string email);

    public Task<Result<Credentials>>
        Login(string email, string code, UserAgentInfo userAgentInfo);
}

public class TemporaryCodeService(
    AuthDbContext db,
    IPasswordHasherService passwordHasherService,
    IEmailSender emailSender,
    ILoginAttemptService loginAttemptService,
    IRefreshTokenService refreshTokenService,
    IJwtService jwtService) : ITemporaryCodeService
{
    public async Task GenerateAndSendCode(string email)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return;
        }

        var code = TokenGenerator.GenerateOtp();
        user.TemporaryPasswordHash = passwordHasherService.Hash(code);
        user.TemporaryPasswordExpiresAt = DateTime.UtcNow.AddMinutes(3);
        
        // TODO We should outbox here too to garantee consistency
        await db.SaveChangesAsync();
        
        await emailSender.SendEmailAsync(user.Email, "Temporary Code", code);
    }

    public async Task<Result<Credentials>> 
        Login(string email, string code, UserAgentInfo userAgentInfo)
    {
        var user = await db.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
        if (user == null)
        {
            await loginAttemptService.Register(email, userAgentInfo.IpAddress, false);
            return Result<Credentials>.Failure("Invalid email", ErrorType.Forbidden);
        }

        if (user.TemporaryPasswordExpiresAt < DateTime.UtcNow)
        {
            await loginAttemptService.Register(email, userAgentInfo.IpAddress, false);
            return Result<Credentials>.Failure("Code expired", ErrorType.Forbidden);
        }

        var isCodeCorrect = passwordHasherService
            .Verify(user.TemporaryPasswordHash ?? string.Empty, code);
        if (!isCodeCorrect)
        {
            await loginAttemptService.Register(email, userAgentInfo.IpAddress, false);
            return Result<Credentials>.Failure("Invalid code", ErrorType.Forbidden);
        }
        var tx = await db.Database.BeginTransactionAsync();
        
        await loginAttemptService.Register(email, userAgentInfo.IpAddress, true);
        
        user.TemporaryPasswordExpiresAt = DateTime.MinValue;
        user.TemporaryPasswordHash = null;

        try
        {
            await db.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            await tx.RollbackAsync();
            return Result<Credentials>.Failure("Code already used", ErrorType.Forbidden);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return Result<Credentials>.Failure(ex.Message);
        }
        
        var refreshToken = await refreshTokenService.Create(user.Id, userAgentInfo);
        var accessToken = jwtService.Generate(user.Id, email);
        
        return Result<Credentials>.Success(new Credentials(accessToken, refreshToken.Token));
    }
}