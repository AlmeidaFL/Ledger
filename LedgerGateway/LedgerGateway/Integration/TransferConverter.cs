using FinancialService;
using LedgerGateway.Dtos;

namespace LedgerGateway.Integration;

public static class TransferConverter
{
    public static TransferRequest ToGrpc(this TransferRequestDto dto)
        => new TransferRequest
        {
            FromAccountId = dto.FromAccountId,
            ToAccountId = dto.ToAccountId,
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

        response.Result = ResultConverter.ToDto(grpc.Result, response);
        return response;
    }
}