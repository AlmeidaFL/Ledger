using ServiceCommons;
using Result = FinancialService.Result;

namespace LedgerGateway.Integration;

public static class ResultConverter
{
    public static ServiceCommons.Result ToDto(Result grpc, object? value = null)
    {
        return (grpc?.IsSuccess ?? false) 
            ? ServiceCommons.Result.Success(value) 
            : ServiceCommons.Result.Failure(
                error: grpc?.ErrorMessage ?? string.Empty,
                errorType: ParseErrorType(grpc?.ErrorType ?? string.Empty));
    }

    private static ErrorType? ParseErrorType(string errorType)
    {
        return Enum.TryParse<ErrorType>(errorType, out var result) ? result : null;
    }
}