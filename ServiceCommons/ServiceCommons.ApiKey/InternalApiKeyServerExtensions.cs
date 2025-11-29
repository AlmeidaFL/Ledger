using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceCommons.ApiKey;

public static class InternalApiKeyServerExtensions
{
    public static AuthenticationBuilder AddInternalApiKeyAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<InternalApiSettings>(configuration.GetSection("InternalApiSettings"));

        const string scheme = "InternalApiKey";

        return services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = scheme;
                options.DefaultChallengeScheme = scheme;
            })
            .AddScheme<AuthenticationSchemeOptions, InternalApiKeyAuthenticationHandler>(
                scheme, _ => { });
    }
}