using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ServiceCommons.Jwt;

public interface IJwtTokenGenerator
{
    string GenerateUserToken(
        Guid userId,
        string? email = null,
        string? name = null,
        IEnumerable<string>? roles = null,
        IEnumerable<Claim>? customClaims = null);

    string GenerateServiceToken(
        string serviceName,
        TimeSpan? lifetime = null,
        IEnumerable<Claim>? customClaims = null);
}

internal sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions options;
    private readonly byte[] key;

    public JwtTokenGenerator(IOptions<JwtOptions> options)
    {
        this.options = options.Value;
        key = Encoding.UTF8.GetBytes(this.options.Secret);
    }

    public string GenerateUserToken(
        Guid userId,
        string? email = null,
        string? name = null,
        IEnumerable<string>? roles = null,
        IEnumerable<Claim>? customClaims = null)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (email is not null) claims.Add(new Claim(JwtRegisteredClaimNames.Email, email));
        if (name is not null) claims.Add(new Claim("name", name));

        if (roles is not null)
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        if (customClaims is not null)
            claims.AddRange(customClaims);

        return CreateToken(claims, options.AccessTokenExpiration);
    }

    public string GenerateServiceToken(
        string serviceName,
        TimeSpan? lifetime = null,
        IEnumerable<Claim>? customClaims = null)
    {
        var claims = new List<Claim>
        {
            new("svc", serviceName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (customClaims is not null)
            claims.AddRange(customClaims);

        var minutes = (int)(lifetime?.TotalMinutes ?? 5);
        return CreateToken(claims, minutes);
    }

    private string CreateToken(IEnumerable<Claim> claims, int minutes)
    {
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(minutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}