using Microsoft.AspNetCore.Mvc;

namespace ServiceCommons;

public static class ResultExtensions
{
    public static ActionResult FromResult(this ControllerBase controller, Result result)
    {
        if (!result.IsSuccess) return controller.MapError(result);
        
        if (result.Value == null)
        {
            return controller.Ok();
        }
        
        return controller.Ok(result.Value);

    }

    public static ActionResult FromResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value);

        return controller.MapError(result);
    }

    private static ActionResult MapError(this ControllerBase controller, Result result)
    {
        return result.ErrorType switch
        {
            ErrorType.NotFound   => controller.NotFound(result.Error),
            ErrorType.Conflict   => controller.Conflict(result.Error),
            ErrorType.Forbidden  => controller.Forbid(),
            _                    => controller.BadRequest(result.Error)
        };
    }
}