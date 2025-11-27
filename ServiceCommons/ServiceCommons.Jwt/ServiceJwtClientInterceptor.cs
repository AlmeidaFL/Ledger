using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ServiceCommons.Jwt;

public class ServiceJwtClientInterceptor(
    IJwtTokenGenerator tokenGenerator,
    string serviceName) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = tokenGenerator.GenerateServiceToken(serviceName: serviceName);
        request.Headers.Add("Authorization", $"Bearer {token}");
        return base.SendAsync(request, cancellationToken);
    }
}