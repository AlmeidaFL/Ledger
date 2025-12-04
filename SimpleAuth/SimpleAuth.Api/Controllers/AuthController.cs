using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceCommons;
using SimpleAuth.Api.Converters;
using SimpleAuth.Api.Dtos;
using SimpleAuth.Api.Services;
using RegisterRequest = SimpleAuth.Api.Dtos.RegisterRequest;

namespace SimpleAuth.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(
    ILoginService loginService,
    IRefreshTokenService refreshTokenService,
    ITemporaryCodeService temporaryCodeService,
    GoogleAuthService googleAuthService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
    public async Task<ActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        var result = await loginService.Register(registerRequest);
        return this.FromResult(Result<UserResponse>.Combine(result, UserResponseConverter.Convert));
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Credentials))]
    public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
    {        
        var result = await loginService.Login(loginRequest);

        return this.FromResult(result);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Credentials))]
    public async Task<ActionResult> Refresh([FromBody] RefreshRequest refreshRequest)
    {
        var result = await loginService.Refresh(refreshRequest);
        return this.FromResult(result);
    }

    // [Authorize]
    // [HttpGet("sessions/{id:guid}")]
    // public async Task<IActionResult> GetSessions(string email)
    // {
    //     var sessions = await db.RefreshTokens
    //         .Where(x => x.UserId == id && !x.Revoked && x.ExpiresAt > DateTime.UtcNow)
    //         .OrderByDescending(x => x.LastUsedAt)
    //         .Select(x => new
    //         {
    //             id = x.Id,
    //             clientName = x.ClientName,
    //             deviceFamily = x.DeviceFamily,
    //             deviceBrand = x.DeviceBrand,
    //             browserFamily = x.BrowserFamily,
    //             browserVersion = x.BrowserVersion,
    //             ipAddress = x.IpAddress,
    //             createdAt = x.CreatedAt,
    //             lastUsedAt = x.LastUsedAt,
    //             expiresAt = x.ExpiresAt,
    //         })
    //         .ToListAsync();
    //     
    //     return Ok(sessions);
    // }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]

    public async Task<ActionResult> Logout([FromQuery] string email, [FromBody] LogoutRequest request)
    {
        await refreshTokenService.RevokeToken(email, request.RefreshToken);
        return Ok();
    }
    
    [Authorize]
    [HttpPost("logout-all")]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> LogoutAll([FromQuery] string email)
    {
        await refreshTokenService.RevokeAllTokens(email);
        return Ok();
    }

    [HttpPost("request-temp-code")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TemporaryCodeResponse))]
    public async Task<ActionResult> RequestTemporaryCode([FromQuery] string email)
    {
        await temporaryCodeService.GenerateAndSendCode(email);
        return Ok(new TemporaryCodeResponse("If this email exists an code was sent to it"));
    }
    
    [HttpPost("login-temp")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Credentials))]
    public async Task<ActionResult> LoginWithTemporaryCode([FromBody] LoginWithTemporaryCodeRequest request)
    {
        var result = await temporaryCodeService.Login(request.Email, request.Code, request.UserAgentInfo);

        return this.FromResult(result);
    }

    [HttpPost("google")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Credentials))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Credentials>> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        var result = await googleAuthService.Login(request.IdToken, request.UserAgentInfo);
        return this.FromResult(result);
    }
}