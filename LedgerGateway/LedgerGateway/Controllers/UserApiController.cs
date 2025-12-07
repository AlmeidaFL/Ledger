using System.Security.Claims;
using LedgerGateway.RestClients.UserApi;
using Microsoft.AspNetCore.Mvc;
using ServiceCommons;

namespace LedgerGateway.Controllers;

[ApiController]
[Route("api/users")]
public class UserApiController(UserApiClient client) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> CreateUser(CreateUserRequest createUserRequest, CancellationToken cancellationToken = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.UserPOSTAsync(createUserRequest, cancellationToken)
        );

        return this.FromResult(result);
    }
    
    [HttpGet]
    public async Task<ActionResult> GetUser([FromQuery] string? email, CancellationToken ct)
    {
        var isSpaClient = Request.Headers.TryGetValue("X-Client-Type", out var value)
                          && value == "spa";
        
        var emailValue = isSpaClient && email is null ?
            User.FindFirstValue(ClaimTypes.Email) : email;
        
        var result = await RestSafeCaller.Call(() =>
            client.UserGETAsync(email, ct)
        );

        return this.FromResult(result);
    }
    
    [HttpPut]
    public async Task<ActionResult> UpdateUser([FromQuery] string email, [FromBody] UpdateUserRequest request, CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.UserPUTAsync(email, request, ct)
        );

        return this.FromResult(result);
    }
    
    [HttpDelete]
    public async Task<ActionResult> Delete([FromQuery] string email, CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.UserDELETEAsync(email, ct)
        );

        return this.FromResult(result);
    }
}
