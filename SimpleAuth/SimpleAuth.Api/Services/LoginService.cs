using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServiceCommons;
using SimpleAuth.Api.Dtos;
using SimpleAuth.Api.Model;
using SimpleAuth.Api.Repository;

namespace SimpleAuth.Api.Services;

public interface ILoginService
{
    public Task<Result<User>> Register(RegisterRequest request);
    public Task<Result<Credentials>> Login(LoginRequest request);
    
    public Task<Result<Credentials>> Refresh(RefreshRequest request);
}

public class LoginService(
    AuthDbContext db,
    IPasswordHasherService hasher, 
    ILoginAttemptService loginAttemptService,
    IJwtService jwtService,
    IRefreshTokenService refreshTokenService) : ILoginService
{
    public async Task<Result<User>> Register(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLower();
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = hasher.Hash(request.Password),
        };
        
        try
        {
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") ?? false)
        {
            return Result<User>.Failure("Email already exists", ErrorType.Conflict);
        }

        return Result<User>.Success(user);
    }

    public async Task<Result<Credentials>> Login(LoginRequest loginRequest)
    {
        var email = loginRequest.Email.Trim().ToLower();
        
        if (await loginAttemptService.IsBlocked(email, loginRequest.UserAgentInfo.IpAddress))
        {
            return Result<Credentials>.Failure("Too many attempts. Please try again later.", ErrorType.TooManyAttempts);
        }
        
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            await loginAttemptService.Register(email, loginRequest.UserAgentInfo.IpAddress, success: false);
            return Result<Credentials>.Failure("Invalid credentials", ErrorType.NotFound);
        }

        if (!hasher.Verify(user.PasswordHash, loginRequest.Password))
        {
            await loginAttemptService.Register(email, loginRequest.UserAgentInfo.IpAddress, success: false);
            return Result<Credentials>.Failure("Invalid credentials", ErrorType.Forbidden);
        }

        await loginAttemptService.Register(email, loginRequest.UserAgentInfo.IpAddress, success: true);
        
        var accessToken = jwtService.Generate(user.Id, user.Email);
        var refresh = await refreshTokenService.Create(
            userId: user.Id,
            userAgentInfo: loginRequest.UserAgentInfo);
        
        return Result<Credentials>.Success(new Credentials(accessToken, refresh.Token));
    }

    public async Task<Result<Credentials>> Refresh(RefreshRequest request)
    {
        var oldToken = await refreshTokenService.GetSessionByToken(request.RefreshToken);

        if (oldToken == null)
        {
            return Result<Credentials>.Failure("Invalid refresh token", ErrorType.NotFound);
        }

        if (oldToken.Revoked)
        {
            return Result<Credentials>.Failure("Refresh token revoked", ErrorType.Forbidden);
        }

        if (oldToken.ExpiresAt < DateTime.UtcNow)
        {
            return  Result<Credentials>.Failure("Refresh token has expired", ErrorType.Forbidden);
        }

        var result = await refreshTokenService.Rotate(oldToken, request.UserAgentInfo);
        if (result.IsFailure)
        {
            return Result<Credentials>.Failure(result: result);
        }

        var newToken = result.Value!;
        var accessToken = jwtService.Generate(newToken.UserId, oldToken.User.Email);
        
        return Result<Credentials>.Success(new Credentials(accessToken, newToken.Token));
    }
}