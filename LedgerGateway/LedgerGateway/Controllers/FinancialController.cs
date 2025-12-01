using LedgerGateway.Integration;
using IResult = LedgerGateway.Dtos.IResult;

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
        var result = await SafeCall(async () =>
        {
            var response = await client.DepositAsync(request, cancellationToken: ct);
            return response.ToDto();
        });

        return this.FromResult(result);
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> TransferAsync([FromBody] TransferRequest request, CancellationToken ct)
    {
        var result = await SafeCall(async () =>
        {
            var response = await client.TransferAsync(request, cancellationToken: ct);
            return response.ToDto();
        });

        return this.FromResult(result);
    }

    private static async Task<ServiceCommons.Result> SafeCall<T>(Func<Task<T>> action)
        where T : IResult
    {
        try
        {
            var data = await action();
            return data.Result;
        }
        catch (Grpc.Core.RpcException rpcEx)
        {
            return Result<T>.Failure(
                rpcEx.Message ?? "Unexpected server error",
                ErrorType.Unexpected);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(
                ex.Message ?? "Unexpected error",
                ErrorType.Unexpected);
        }
    }
}
