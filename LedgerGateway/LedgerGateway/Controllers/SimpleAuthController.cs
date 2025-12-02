using LedgerGateway.RestClients.SimpleAuth;

namespace LedgerGateway.Controllers;
using Microsoft.AspNetCore.Mvc;
using ServiceCommons;
using System.Threading;
using System.Threading.Tasks;

[ApiController]
[Route("api/simple-auth")]
public class SimpleAuthController(SimpleAuthClient client) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.RegisterAsync(request, ct)
        );

        return this.FromResult(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.LoginAsync(request, ct)
        );

        return this.FromResult(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh(
        [FromBody] RefreshRequest request,
        CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.RefreshAsync(request, ct)
        );

        return this.FromResult(result);
    }

    [HttpPost("logout/{id:guid}")]
    public async Task<ActionResult> Logout(
        Guid id,
        [FromBody] LogoutRequest request,
        CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.LogoutAsync(id, request, ct)
        );

        return this.FromResult(result);
    }

    [HttpPost("logout-all/{id:guid}")]
    public async Task<ActionResult> LogoutAll(
        Guid id,
        CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.LogoutAllAsync(id, ct)
        );

        return this.FromResult(result);
    }

    [HttpPost("request-temp-code/{id:guid}")]
    public async Task<ActionResult> RequestTemporaryCode(
        Guid id,
        CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.RequestTempCodeAsync(id, ct)
        );

        return this.FromResult(result);
    }

    [HttpPost("login-temp")]
    public async Task<ActionResult> LoginWithTemporaryCode(
        [FromBody] LoginWithTemporaryCodeRequest request,
        CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.LoginTempAsync(request, ct)
        );

        return this.FromResult(result);
    }

    [HttpPost("google")]
    public async Task<ActionResult> GoogleLogin(
        [FromBody] GoogleLoginRequest request,
        CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.GoogleAsync(request, ct)
        );

        return this.FromResult(result);
    }
}
