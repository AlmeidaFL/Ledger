using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceCommons.Jwt;

namespace ServiceCommons.Grpc;

public static class GrpcAuthenticationExtensions
{
    public static IServiceCollection AddGrpcAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddJwtTokenGenerator();
        return services;
    }
    
    public static IHttpClientBuilder AddServiceJwtGrpcInterceptor<TClient>(
        this IHttpClientBuilder builder,
        string serviceName)
        where TClient : class
    {
        return builder.AddInterceptor(sp =>
        {
            var generator = sp.GetRequiredService<IJwtTokenGenerator>();
            return new ServiceJwtGrpcInterceptor(generator, serviceName);
        });
    }
}