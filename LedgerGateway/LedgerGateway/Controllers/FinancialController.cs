using LedgerGateway.Application;
using LedgerGateway.Converters;
using LedgerGateway.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace LedgerGateway.Controllers;

using FinancialService;
using Microsoft.AspNetCore.Mvc;
using ServiceCommons;

[ApiController]
[Route("api/financial")]
public class FinancialController(FinancialService.FinancialServiceClient client) : ControllerBase
{
    [Authorize]
    [HttpGet("test2")]
    public async Task<IActionResult> Test2()
    {
        return Ok();
    }
    
    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        return Ok(new
        {
            message = await client.TestAsync(new TestRequest{ Testou = "sim" })
        });
    }
    
    
    [HttpPost("deposit")]
    public async Task<IActionResult> DepositAsync([FromBody] DepositRequestDto request, CancellationToken ct)
    {
        request.UserEmail = User.GetEmailOrThrow();
        var result = await GrpcSafeCaller.Call(async () =>
        {
            var response = await client.DepositAsync(request.ToGrpc(), cancellationToken: ct);
            return ResultConverter.Convert(response.Result, response.ToDto());
        });

        return this.FromResult(result);
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> TransferAsync([FromBody] TransferRequestDto request,  CancellationToken ct)
    {
        request.FromAccountEmail = User.GetEmailOrThrow();
        var result = await GrpcSafeCaller.Call(async () =>
        {
            var response = await client.TransferAsync(request.ToGrpc(), cancellationToken: ct);
            return ResultConverter.Convert(response.Result, response.ToDto());
        });

        return this.FromResult(result);
    }
    
    [HttpGet("balance")]
    public async Task<IActionResult> GetBalanceAsync(CancellationToken ct)
    {
        var request = new GetBalanceRequest
        {
            UserEmail = User.GetEmailOrThrow()
        };
        var result = await GrpcSafeCaller.Call(async () =>
        {
            var response = await client.GetBalanceAsync(request, cancellationToken: ct);
            return ResultConverter.Convert(response.Result, response.ToDto());
        });

        return this.FromResult(result);
    }
}
