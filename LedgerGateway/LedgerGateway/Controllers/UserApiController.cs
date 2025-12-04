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
            client.UserAsync(createUserRequest, cancellationToken)
        );

        return this.FromResult(result);
    }
    
    [HttpGet("userEmail")]
    public async Task<ActionResult> GetUser(string userEmail, CancellationToken ct)
    {
        var result = await RestSafeCaller.Call(() =>
            client.UserEmailGETAsync(userEmail, ct)
        );

        return this.FromResult(result);
    }
    
    [HttpPut("userEmail")]
    public async Task<ActionResult> UpdateUser(string userEmail, [FromBody] UpdateUserRequest request, CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.UserEmailPUTAsync(userEmail, request, ct)
        );

        return this.FromResult(result);
    }
    
    [HttpDelete("userEmail")]
    public async Task<ActionResult> Delete(string userEmail, CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.UserEmailDELETEAsync(userEmail, ct)
        );

        return this.FromResult(result);
    }
}
