using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LedgerGateway;

public static class ClientAuthExtensions
{
    public static void AddClientAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var key = configuration.GetSection("Jwt:Key")?.Value
            ?? throw new Exception("Jwt issuer is missing");
        var issuer = configuration.GetSection("Jwt:Issuer")?.Value
            ?? throw new Exception("Jwt issuer is missing");
        var audience = configuration.GetSection("Jwt:Audience")?.Value
            ?? throw new Exception("Jwt audience is missing");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
                        if (!string.IsNullOrEmpty(authHeader) &&
                            authHeader.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase))
                        {
                            context.Token = authHeader["Bearer ".Length..];
                            return Task.CompletedTask;
                        }

                        var cookieToken = context.Request.Cookies["access_token"];
                        if (!string.IsNullOrEmpty(cookieToken))
                        {
                            context.Token = cookieToken;
                        }

                        return Task.CompletedTask;
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
    }
}