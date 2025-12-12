using FinancialService;
using LedgerGateway.Dtos;

namespace LedgerGateway.Converters;

public static class TransferConverter
{
    public static TransferRequest ToGrpc(this TransferRequestDto dto)
        => new TransferRequest
        {
            FromAccountEmail = dto.FromUserEmail,
            ToAccountEmail = dto.ToUserEmail,
            Amount = dto.Amount,
            Currency = dto.Currency,
            IdempotencyKey = dto.IdempotencyKey,
            Metadata = dto.Metadata ?? string.Empty
        };

    public static TransferResponseDto ToDto(this TransferResponse grpc)
    {
        var response = new TransferResponseDto
        {
            TransactionId = grpc.TransactionId,
            Status = grpc.Status,
            IsIdempotentReplay = grpc.IsIdempotentReplay,
        };

        return response;
    }
}