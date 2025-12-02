using Microsoft.EntityFrameworkCore;
using ServiceCommons;
using SimpleAuth.Api.Data;

namespace SimpleAuth.Api.Services;

public interface IRefreshTokenService
{
    Task<RefreshToken> Create(
        Guid userId,
        UserAgentInfo userAgentInfo);
    Task<RefreshToken?> GetByToken(string token);
    Task<Result<RefreshToken>> Rotate(RefreshToken oldToken, UserAgentInfo userAgentInfo);
    Task<Result> RevokeToken(Guid userId, string token);
    Task<Result> RevokeAllTokens(Guid userId);
}

public class RefreshTokenService(AuthDbContext dbContext): IRefreshTokenService
{
    public async Task<RefreshToken> Create(
        Guid userId,
        UserAgentInfo userAgentInfo)
    {
        var token = TokenGenerator.GenerateRefreshToken();

        var refresh = new RefreshToken()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Revoked = false,
            
            IpAddress = userAgentInfo.IpAddress,
            UserAgent = userAgentInfo.UserAgent,
            
            DeviceFamily = userAgentInfo.DeviceFamily,
            DeviceBrand = userAgentInfo.DeviceBrand,
            BrowserFamily = userAgentInfo.BrowserFamily,
            BrowserVersion = userAgentInfo.BrowserVersion,
            ClientName = userAgentInfo.ClientName,
            ClientType = userAgentInfo.ClientType,
        };
        
        await dbContext.RefreshTokens.AddAsync(refresh);
        await dbContext.SaveChangesAsync();
        
        return refresh;
    }

    public async Task<RefreshToken?> GetByToken(string token)
    {
        return await dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == token);
    }

    public async Task<Result<RefreshToken>> Rotate(RefreshToken oldToken,  UserAgentInfo userAgentInfo)
    {
        oldToken.Revoked = true;
        oldToken.IpAddress = userAgentInfo.IpAddress;
        oldToken.UserAgent = userAgentInfo.UserAgent;
        
        oldToken.DeviceFamily = userAgentInfo.DeviceFamily;
        oldToken.DeviceBrand = userAgentInfo.DeviceBrand;
        oldToken.BrowserFamily = userAgentInfo.BrowserFamily;
        oldToken.BrowserVersion = userAgentInfo.BrowserVersion;
        oldToken.ClientName = userAgentInfo.ClientName;
        oldToken.ClientType = userAgentInfo.ClientType;

        var newToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = oldToken.UserId,
            Token = TokenGenerator.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Revoked = false,
        };
        
        await dbContext.RefreshTokens.AddAsync(newToken);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<RefreshToken>.Failure("Refresh token already used", ErrorType.Conflict);
        }
        catch (Exception ex)
        {
            return Result<RefreshToken>.Failure(ex.Message);
        }

        return Result<RefreshToken>.Success(newToken);
    }
    
    public async Task<Result> RevokeToken(Guid userId, string token)
    {
        var session = await GetByToken(token);
        if (session == null)
        {
            return Result.Failure("Invalid token", ErrorType.NotFound);
        }
        
        if (session.UserId != userId)
        {
            return Result.Failure("Invalid user id", ErrorType.NotFound);
        }
        
        session.Revoked = true;
        await dbContext.SaveChangesAsync();
        
        return Result.Success();
    }

    public async Task<Result> RevokeAllTokens(Guid userId)
    {
        await dbContext.RefreshTokens
            .Where(r => r.UserId == userId && !r.Revoked)
            .ForEachAsync(r =>
            {
                r.Revoked = true;
            });

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Failure("Refresh token already revoked", ErrorType.Conflict);
        }
        catch (Exception ex)
        {
            return Result<RefreshToken>.Failure(ex.Message);
        }
        
        return Result.Success();
    }
}