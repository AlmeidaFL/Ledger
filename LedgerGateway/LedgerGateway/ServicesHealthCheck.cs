using LedgerGateway.RestClients.UserApi;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LedgerGateway;

public class ServicesHealthCheck(IConfiguration configuration) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var httpClient = new HttpClient();
        
        var userApi = GetUrlOrThrow("UserApi");
        var userApiResult = await httpClient.GetAsync(userApi, cancellationToken);
        
        var financialService = GetUrlOrThrow("FinancialService");
        var financialServiceResult = await httpClient.GetAsync(financialService, cancellationToken);
        
        var simpleAuth = GetUrlOrThrow("SimpleAuth");
        var simpleAuthResult = await httpClient.GetAsync(simpleAuth, cancellationToken);

        if (!(userApiResult.IsSuccessStatusCode
              || financialServiceResult.IsSuccessStatusCode
              || simpleAuthResult.IsSuccessStatusCode))
        {
            return new HealthCheckResult(
                status: HealthStatus.Unhealthy,
                description: $"""
                              userApi: {IsHealthy(userApiResult.IsSuccessStatusCode)}
                              financialService: {IsHealthy(financialServiceResult.IsSuccessStatusCode)}
                              simpleAuth: {IsHealthy(simpleAuthResult.IsSuccessStatusCode)}
                              """);
        }

        return new HealthCheckResult(HealthStatus.Healthy);
    }
    
    private string GetUrlOrThrow(string serviceName)
    {
        var url = configuration.GetSection(serviceName).GetValue<string>("BaseUrl") 
               ?? throw new InvalidOperationException($"{serviceName}.BaseUrl not configured.");
        
        return $"{url}/health";        
    }

    private static string IsHealthy(bool isHealthy)
    {
        return isHealthy ? "healthy" : "unhealthy";
    }
}