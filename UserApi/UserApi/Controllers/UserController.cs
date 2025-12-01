using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceCommons;
using UserApi.Converters;
using UserApi.Dtos;
using UserApi.Services;

namespace UserApi.Controllers;

[Authorize]
[ApiController]
[Route("user")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResponse))]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request,
        CancellationToken cancelationToken = default)
    {
        var result = await userService.CreateUserAsync(request, cancelationToken);

        return result.IsFailure
            ? this.FromResult(result)
            : CreatedAtAction(nameof(GetUser), new { userId = result.Value!.Id }, result.Value);
    }

    [HttpGet("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResponse))]
    public async Task<IActionResult> GetUser(Guid userId, CancellationToken cancelationToken = default)
    {
        var requestingUserId = GetRequestingUserId();
        var result = await userService.GetUserAsync(userId, requestingUserId, cancelationToken);
        return this.FromResult(result);
    }
    
    [HttpPut("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResponse))]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var requestingUserId = GetRequestingUserId();
        var result = await userService.UpdateUserAsync(userId, requestingUserId, request, cancellationToken);

        return this.FromResult(result);
    }
    
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateUser(Guid userId, CancellationToken ct)
    {
        var requestingUserId = GetRequestingUserId();

        var result = await userService.DeactivateUserAsync(userId, requestingUserId, ct);

        return result.IsFailure ? this.FromResult(result) : NoContent();
    }
    
    private Guid GetRequestingUserId()
    {
        var header = HttpContext.Request.Headers["X-User-Id"].FirstOrDefault();
        return Guid.TryParse(header, out var value) ? value : Guid.Empty;
    }
}