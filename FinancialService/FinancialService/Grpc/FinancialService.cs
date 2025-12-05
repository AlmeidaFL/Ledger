using FinancialService.Application.Services;
using Grpc.Core;
using Microsoft.AspNetCore.Connections.Features;
using ServiceCommons;

namespace FinancialService.Grpc;

public class FinancialService(
    IDepositService depositService,
    ITransferService transferService) 
    : global::FinancialService.FinancialService.FinancialServiceBase
{
    public override Task<TestRequest> Test(TestRequest request, ServerCallContext context)
    {
        var http = context.GetHttpContext();

        var protocol = http.Request.Protocol;
        var isHttps = http.Request.IsHttps;
        var hasTls = http.Features.Get<ITlsHandshakeFeature>() != null;

        Console.WriteLine($"Protocol negotiated: {protocol}");
        Console.WriteLine($"IsHttps: {isHttps}");
        Console.WriteLine($"TLS handshake feature present: {hasTls}");
        
        Console.WriteLine($"[gRPC SERVER] Protocol negotiated: {protocol}");
        return Task.FromResult(request);
    }

    public override async Task<DepositResponse> Deposit(DepositRequest request, ServerCallContext context)
    {
        var result = await depositService.DepositAsync(
            userEmail: request.UserEmail,
            amount: request.Amount,
            currency: request.Currency,
            idempotencyKey: request.IdempotencyKey,
            context.CancellationToken);

        var status = result.IsSuccess ? "success" : string.Empty;
        return new DepositResponse
        {
            TransactionId = result.Value?.TransactionId.ToString() ?? string.Empty,
            Status = status,
            Result = new Result
            {
                ErrorMessage = result.Error ?? string.Empty,
                IsSuccess = result.IsSuccess,
                ErrorType = result.ErrorType?.ToString() ?? string.Empty,
            }
        };
    }

    public override async Task<TransferResponse> Transfer(TransferRequest request, ServerCallContext context)
    {
        var result = await transferService.TransferAsync(
            fromAccountEmail: request.FromAccountEmail,
            toAccountEmail: request.ToAccountEmail,
            amount: request.Amount,
            currency: request.Currency,
            idempotencyKey: request.IdempotencyKey,
            metadata: request.Metadata,
            context.CancellationToken);
        
        var status = result.IsSuccess ? "success" : String.Empty;
        return new TransferResponse
        {
            TransactionId = result.Value?.TransactionId.ToString() ?? string.Empty,
            Status = status,
            IsIdempotentReplay = result.Value?.IsIdempotentReplay ?? false,
            Result = new Result
            {
                ErrorMessage = result.Error ?? string.Empty,
                IsSuccess = result.IsSuccess,
                ErrorType = result.ErrorType?.ToString() ?? string.Empty,
            }
        };
    }
}