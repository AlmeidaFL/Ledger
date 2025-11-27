using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ServiceCommons.Jwt;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        services.Configure<JwtOptions>(jwtSection);
        services.Configure<ServiceAuthOptions>(configuration.GetSection(ServiceAuthOptions.SectionName));

        var jwtOptions = jwtSection.Get<JwtOptions>()
                ?? throw new InvalidOperationException("Jwt configuration section missing.");
        var key = Encoding.UTF8.GetBytes(jwtOptions.Secret);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(jwtOptions.ClockSkewMinutes)
                };

                options.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = context =>
                    {
                        var svc = context.Principal?.FindFirst("svc")?.Value;
                        // User token
                        if (svc is null)
                        {
                            return Task.CompletedTask;
                        }

                        var serviceOptions =
                            context.HttpContext.RequestServices.GetService<IOptions<ServiceAuthOptions>>();
                        if (!serviceOptions?.Value.AllowedServices.Contains(svc) ?? false)
                        {
                            context.Fail($"Service '{svc}' is not allowed.");
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, HttpContextUser>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
    
        public static IServiceCollection AddJwtTokenGenerator(
            this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}