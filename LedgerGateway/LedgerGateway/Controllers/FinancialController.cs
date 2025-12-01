using LedgerGateway.Integration;

namespace LedgerGateway.Controllers;

using FinancialService;
using Microsoft.AspNetCore.Mvc;
using ServiceCommons;

[ApiController]
[Route("api/financial")]
public class FinancialController(FinancialService.FinancialServiceClient client) : ControllerBase
{
    [HttpPost("deposit")]
    public async Task<IActionResult> DepositAsync([FromBody] DepositRequest request, CancellationToken ct)
    {
        var result = await GrpcSafeCaller.Call(async () =>
        {
            var response = await client.DepositAsync(request, cancellationToken: ct);
            return ResultConverter.ToDto(response.Result, response.ToDto());
            
        });

        return this.FromResult(result);
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> TransferAsync([FromBody] TransferRequest request,  CancellationToken ct)
    {
        var result = await GrpcSafeCaller.Call(async () =>
        {
            var response = await client.TransferAsync(request, cancellationToken: ct);
            return ResultConverter.ToDto(response.Result, response.ToDto());
        });

        return this.FromResult(result);
    }
}
