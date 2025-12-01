using FinancialService.Application.Services;
using Grpc.Core;

namespace FinancialService.Grpc;

public class FinancialService(IDepositService depositService) 
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
}