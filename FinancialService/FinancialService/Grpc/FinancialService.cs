using FinancialService.Application.Services;
using Grpc.Core;

namespace FinancialService.Grpc;

public class FinancialService(
    IDepositService depositService,
    ITransferService transferService) 
    : global::FinancialService.FinancialService.FinancialServiceBase
{
    public override async Task<DepositResponse> Deposit(DepositRequest request, ServerCallContext context)
    {
        var result = await depositService.DepositAsync(
            userId: Guid.Parse(request.UserId),
            amount: request.Amount,
            currency: request.Currency,
            idempotencyKey: request.IdempotencyKey,
            context.CancellationToken);

        var status = result.IsSuccess ? "success" : result.Error;
        return new DepositResponse
        {
            TransactionId = result.Value?.TransactionId.ToString() ?? string.Empty,
            Status = status,
        };
    }

    public override async Task<TransferResponse> Transfer(TransferRequest request, ServerCallContext context)
    {
        var result = await transferService.TransferAsync(
            fromAccountId: Guid.Parse(request.FromAccountId),
            toAccountId: Guid.Parse(request.ToAccountId),
            amount: request.Amount,
            currency: request.Currency,
            idempotencyKey: request.IdempotencyKey,
            metadata: request.Metadata,
            context.CancellationToken);
        
        var status = result.IsSuccess ? "success" : result.Error;
        return new TransferResponse
        {
            TransactionId = result.Value?.TransactionId.ToString() ?? string.Empty,
            Status = status,
            IsIdempotentReplay = result.Value?.IsIdempotentReplay ?? false,
        };
    }
}