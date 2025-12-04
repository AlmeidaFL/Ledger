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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request,
        CancellationToken cancelationToken = default)
    {
        var result = await userService.CreateUserAsync(request, cancelationToken);

        return this.FromResult(result);
    }

    [HttpGet("userEmail")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
    public async Task<IActionResult> GetUser(string userEmail, CancellationToken cancelationToken = default)
    {
        var result = await userService.GetUserAsync(userEmail, cancelationToken);
        return this.FromResult(result);
    }
    
    [HttpPut("userEmail")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
    public async Task<IActionResult> UpdateUser(string userEmail, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await userService.UpdateUserAsync(userEmail, request, cancellationToken);

        return this.FromResult(result);
    }
    
    [HttpDelete("userEmail")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateUser(string userEmail, CancellationToken ct)
    {
        var result = await userService.DeactivateUserAsync(userEmail, ct);

        return this.FromResult(result);
    }
}