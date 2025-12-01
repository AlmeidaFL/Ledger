using System.Text.Json;
using LedgerGateway.RestClients;

namespace LedgerGateway.Controllers;

using ServiceCommons;
using Microsoft.AspNetCore.Mvc;
using System.Net;

public static class RestSafeCaller
{
    public static async Task<Result<T>> Call<T>(Func<Task<T>> action)
    {
        try
        {
            var result = await action();
            return Result<T>.Success(result);
        }
        catch (ApiException apiEx)
        {
            ProblemDetails? problem = null;

            try
            {
                problem = JsonSerializer.Deserialize<ProblemDetails>(apiEx.Response);
            }
            catch
            {
                // ignored
            }

            var errorMessage = problem?.Detail ?? apiEx.Message;

            return apiEx.StatusCode switch
            {
                (int)HttpStatusCode.NotFound =>
                    Result<T>.Failure(errorMessage, ErrorType.NotFound),

                (int)HttpStatusCode.Forbidden =>
                    Result<T>.Failure(errorMessage, ErrorType.Forbidden),

                (int)HttpStatusCode.Conflict =>
                    Result<T>.Failure(errorMessage, ErrorType.Conflict),

                _ =>
                    Result<T>.Failure(errorMessage, ErrorType.Unexpected)
            };
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(ex.Message, ErrorType.Unexpected);
        }
    }
}
