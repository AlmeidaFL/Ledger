using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ServiceCommons.ApiKey;

public class InternalApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock,
    IOptions<InternalApiSettings> settings)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (string.IsNullOrWhiteSpace(settings.Value.ApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Internal API key is missing."));
        }

        if (!Request.Headers.TryGetValue("x-api-key", out var values))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }
        
        var providedApiKey = values.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(providedApiKey)
            || CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(providedApiKey), settings.Value.ApiKeyBytes))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key"));
        }
        
        const string schemeName = "InternalApiKey";
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "internal-client")
        }, schemeName);
        
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, schemeName);
        
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}