using ServiceCommons;
using IResult = LedgerGateway.Dtos.IResult;

namespace LedgerGateway.Controllers;

public static class GrpcSafeCaller
{
    public static async Task<Result> Call<T>(Func<Task<T>> action)
        where T : IResult
    {
        try
        {
            var data = await action();
            return data.Result;
        }
        catch (Grpc.Core.RpcException rpcEx)
        {
            return Result<T>.Failure(
                rpcEx.Message ?? "Unexpected server error",
                ErrorType.Unexpected);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(
                ex.Message ?? "Unexpected error",
                ErrorType.Unexpected);
        }
    }
}