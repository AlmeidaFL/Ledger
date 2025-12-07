using LedgerGateway.Application;
using LedgerGateway.Dtos;
using LedgerGateway.Integration;
using LedgerGateway.RestClients.SimpleAuth;

namespace LedgerGateway.Controllers;
using Microsoft.AspNetCore.Mvc;
using ServiceCommons;
using System.Threading;
using System.Threading.Tasks;

[ApiController]
[Route("api/auth")]
public class SimpleAuthController(
    SimpleAuthClient client,
    IUserAgentParser userAgentParser) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(
        [FromBody] RegisterRequestDto request,
        CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.RegisterAsync(request.ToRegisterRequest(), ct)
        );

        return this.FromResult(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(
        [FromBody] LoginRequestDto dto,
        CancellationToken ct = default)
    {
        var userAgentInfo = GetUserAgentInfoDto().ToUserAgentInfo();
        var loginRequest = dto.Convert();
        loginRequest.UserAgentInfo = userAgentInfo;
    
        var result = await RestSafeCaller.Call(() =>
            client.LoginAsync(loginRequest, ct)
        );

        var isSpaClient = Request.Headers.TryGetValue("X-Client-Type", out var value)
            && value == "spa";

        if (!isSpaClient || result.IsFailure)
        {
            return this.FromResult(result);
        }

        var tokens = result.Value;
        AddCookies(tokens);

        return Ok();
    }

    private void AddCookies(Credentials? tokens)
    {
        Response.Cookies.Append(
            "refresh_token",
            tokens!.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            }
        );
        
        Response.Cookies.Append(
            "access_token",
            tokens!.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            }
        );
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh(
        [FromBody] RefreshRequestDto dto,
        CancellationToken ct = default)
    {
        var request = dto.Convert();
        request.UserAgentInfo = GetUserAgentInfoDto().ToUserAgentInfo();
        var result = await RestSafeCaller.Call(() =>
            client.RefreshAsync(request, ct)
        );

        var isSpaClient = Request.Headers.TryGetValue("X-Client-Type", out var value)
                          && value == "spa";

        if (!isSpaClient || result.IsFailure)
        {
            return this.FromResult(result);
        }

        var tokens = result.Value;
        AddCookies(tokens);

        return Ok();
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout(
        [FromQuery] string userEmail,
        [FromBody] LogoutRequest request,
        CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.LogoutAsync(userEmail, request, ct)
        );

        return this.FromResult(result);
    }

    [HttpPost("logout-all")]
    public async Task<ActionResult> LogoutAll(
        [FromQuery] string userEmail,
        CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.LogoutAllAsync(userEmail, ct)
        );

        return this.FromResult(result);
    }

    // [HttpPost("request-temp-code")]
    // public async Task<ActionResult> RequestTemporaryCode(
    //     string userEmail,
    //     CancellationToken ct = default)
    // {
    //     var result = await RestSafeCaller.Call(() =>
    //         client.RequestTempCodeAsync(userEmail, ct)
    //     );
    //
    //     return this.FromResult(result);
    // }
    //
    // [HttpPost("login-temp")]
    // public async Task<ActionResult> LoginWithTemporaryCode(
    //     [FromBody] LoginWithTemporaryCodeRequestDto dto,
    //     CancellationToken ct = default)
    // {
    //     var request = dto.Convert();
    //     request.UserAgentInfo = GetUserAgentInfoDto().ToUserAgentInfo();
    //     var result = await RestSafeCaller.Call(() =>
    //         client.LoginTempAsync(request, ct)
    //     );
    //
    //     return this.FromResult(result);
    // }
    //
    // [HttpPost("google")]
    // public async Task<ActionResult> GoogleLogin(
    //     [FromBody] GoogleLoginRequestDto dto,
    //     CancellationToken ct = default)
    // {
    //     var request = dto.Convert();
    //     request.UserAgentInfo = GetUserAgentInfoDto().ToUserAgentInfo();
    //     var result = await RestSafeCaller.Call(() =>
    //         client.GoogleAsync(request, ct)
    //     );
    //
    //     return this.FromResult(result);
    // }
    //
    private UserAgentInfoDto GetUserAgentInfoDto()
    {
        var httpContext = HttpContext;

        var ip =
            httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
            ?? httpContext.Connection.RemoteIpAddress?.ToString()
            ?? string.Empty;

        var userAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? string.Empty;

        var parsed = userAgentParser.Parse(userAgent);

        return new UserAgentInfoDto()
        {
            IpAddress = ip,
            UserAgent = userAgent,
            BrowserFamily = parsed.BrowserFamily,
            BrowserVersion = parsed.BrowserVersion,
            DeviceFamily = parsed.DeviceFamily,
            DeviceBrand = parsed.DeviceBrand,
            ClientName = parsed.ClientName,
            ClientType = "Web"
        };
    }
}
