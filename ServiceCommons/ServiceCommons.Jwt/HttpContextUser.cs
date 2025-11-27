using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ServiceCommons.Jwt;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    Guid? UserId { get; }
    string? Email { get; }
    string? Name { get; }
    IList<string> Roles { get; }
    ClaimsPrincipal? Principal { get; }
}

public class HttpContextUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId
    {
        get
        {
            var sub = Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return Guid.TryParse(sub, out var guid) ? guid : default;
        }
    }

    public string? Email =>
        Principal?.FindFirstValue(JwtRegisteredClaimNames.Email);
    
    public string? Name =>
        Principal?.FindFirstValue(JwtRegisteredClaimNames.Name);
    
    public IList<string>  Roles => Principal?.FindAll(ClaimTypes.Role)
        .Select(x => x.Value)
        .ToArray()
        ?? Array.Empty<string>();
    public ClaimsPrincipal? Principal => accessor.HttpContext?.User;
}