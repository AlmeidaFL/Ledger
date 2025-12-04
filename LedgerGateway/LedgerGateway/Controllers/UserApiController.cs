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
    public async Task<ActionResult> GetUser([FromQuery] string userEmail, CancellationToken ct)
    {
        var result = await RestSafeCaller.Call(() =>
            client.UserGETAsync(userEmail, ct)
        );

        return this.FromResult(result);
    }
    
    [HttpPut]
    public async Task<ActionResult> UpdateUser([FromQuery] string userEmail, [FromBody] UpdateUserRequest request, CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.UserPUTAsync(userEmail, request, ct)
        );

        return this.FromResult(result);
    }
    
    [HttpDelete]
    public async Task<ActionResult> Delete([FromQuery] string userEmail, CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.UserDELETEAsync(userEmail, ct)
        );

        return this.FromResult(result);
    }
}
