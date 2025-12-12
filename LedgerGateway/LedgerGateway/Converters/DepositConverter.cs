using FinancialService;
using LedgerGateway.Dtos;

namespace LedgerGateway.Converters;

public static class DepositConverter
{
    public static DepositRequest ToGrpc(this DepositRequestDto dto)
        => new DepositRequest
        {
            UserEmail = dto.UserEmail,
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
        
        return response;
    }
}