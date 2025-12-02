using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceCommons.ApiKey;

public static class InternalApiKeyServerExtensions
{
    public const string Scheme = "InternalApiKey";

    public static AuthenticationBuilder AddInternalApiKeyAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<InternalApiSettings>(configuration.GetSection("InternalApiSettings"));

        return services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = Scheme;
                options.DefaultChallengeScheme = Scheme;
            })
            .AddScheme<AuthenticationSchemeOptions, InternalApiKeyAuthenticationHandler>(
                Scheme, _ => { });
    }
}