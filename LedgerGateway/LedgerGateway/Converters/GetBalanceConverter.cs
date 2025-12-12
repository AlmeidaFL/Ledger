using FinancialService;
using LedgerGateway.Dtos;

namespace LedgerGateway.Converters;

public static class GetBalanceConverter
{
    public static BalanceResponseDto ToDto(this GetBalanceResponse grpc)
    {
        return new BalanceResponseDto
        {
            Amount = grpc.BalanceInCents,
            Currency = grpc.Currency,
            UserEmail = grpc.UserEmail
        };
    }
}