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
    
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult> GetUser(Guid userId, CancellationToken ct)
    {
        var result = await RestSafeCaller.Call(() =>
            client.UserGETAsync(userId, ct)
        );

        return this.FromResult(result);
    }
    
    [HttpPut("{userId:guid}")]
    public async Task<ActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request, CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.UserPUTAsync(userId, request, ct)
        );

        return this.FromResult(result);
    }
    
    [HttpDelete("{userId:guid}")]
    public async Task<ActionResult> Delete(Guid userId, CancellationToken ct = default)
    {
        var result = await RestSafeCaller.Call(() =>
            client.UserDELETEAsync(userId, ct)
        );

        return this.FromResult(result);
    }
}
