using LedgerGateway.RestClients;
using Microsoft.AspNetCore.Mvc;
using ServiceCommons;

namespace LedgerGateway.Controllers;

[ApiController]
[Route("api/users")]
public class UserApiController(UserApiClient client) : ControllerBase
{
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult> GetUser(Guid userId, CancellationToken ct)
    {
        var result = await RestSafeCaller.Call(() =>
            client.UserGETAsync(userId, ct)
        );

        return this.FromResult(result);
    }
}
