using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using ServiceCommons;
using SimpleAuth.Api.Data;
using SimpleAuth.Api.Dtos;

namespace SimpleAuth.Api.Services;

public class GoogleAuthService(
    IConfiguration config,
    IRefreshTokenService refreshTokenService,
    IJwtService jwtService,
    AuthDbContext dbContext)
{
    // Revise this method, isn't idempotent
    public async Task<Result<Credentials>> Login(string idToken, UserAgentInfo userAgentInfo)
    {
        GoogleJsonWebSignature.Payload payload;

        try
        {
            payload = await GetGooglePayload(idToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Result<Credentials>.Failure("Invalid Google Token", ErrorType.Unauthorized);
        }
        
        var email = payload.Email.Trim().ToLowerInvariant();
        var googleId = payload.Subject;
        
        var user = dbContext.Users.FirstOrDefault(u => u.Email == email);

        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                GoogleId = googleId,
            };
            
            await dbContext.Users.AddAsync(user);
        }
        else
        {
            if (string.IsNullOrEmpty(user.GoogleId))
            {
                user.GoogleId = googleId;
            }
        }

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<Credentials>.Failure("User already created", ErrorType.Conflict);
        }
        catch (Exception ex)
        {
            return Result<Credentials>.Failure(ex.Message);
        }
        
        var refreshToken = await refreshTokenService.Create(user.Id, userAgentInfo);
        var accessToken = jwtService.Generate(user.Id, user.Email);

        return Result<Credentials>.Success(new Credentials(accessToken, refreshToken.Token));
    }
    
    private async Task<GoogleJsonWebSignature.Payload> GetGooglePayload(string idToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = [config["Google:ClientId"]],
        };
        
        return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
    }
}