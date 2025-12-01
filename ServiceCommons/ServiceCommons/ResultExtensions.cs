using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ServiceCommons;

public static class ResultExtensions
{
    public static ActionResult FromResult(this ControllerBase controller, Result result)
    {
        if (!result.IsSuccess)
            return controller.MapError(result);

        if (result is Result<object> { Value: null })
            return controller.NoContent();

        return controller.Ok();
    }

    public static ActionResult FromResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (!result.IsSuccess) return controller.MapError(result);
        
        if (result.Value is not null)
            return controller.Ok(result.Value);
        
        return controller.NoContent();
    }

    private static ActionResult MapError(this ControllerBase controller, Result result)
    {
        return result.ErrorType switch
        {
            ErrorType.NotFound => controller.Problem(
                detail: result.Error,
                title: "Not Found",
                statusCode: StatusCodes.Status404NotFound),

            ErrorType.Conflict => controller.Problem(
                detail: result.Error,
                title: "Conflict",
                statusCode: StatusCodes.Status409Conflict),

            ErrorType.Forbidden => controller.Problem(
                detail: result.Error,
                title: "Forbidden",
                statusCode: StatusCodes.Status403Forbidden),

            _ => controller.Problem(
                detail: result.Error,
                title: "Bad Request",
                statusCode: StatusCodes.Status400BadRequest)
        };
    }
}
