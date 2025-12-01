using FinancialService;
using LedgerGateway.Dtos;

namespace LedgerGateway.Integration;

public static class DepositConverter
{
    public static DepositRequest ToGrpc(this DepositRequestDto dto)
        => new DepositRequest
        {
            UserId = dto.UserId,
            Amount = dto.Amount,
            Currency = dto.Currency,
            IdempotencyKey = dto.IdempotencyKey
        };

    public static DepositResponseDto ToDto(this DepositResponse grpc)
    {
        var response = new DepositResponseDto
        {
            TransactionId = grpc.TransactionId,
            Status = grpc.Status,
        };
        
        response.Result = ResultConverter.ToDto(grpc.Result, response);
        return response;
    }
}