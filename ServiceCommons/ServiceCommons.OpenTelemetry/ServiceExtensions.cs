using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ServiceCommons.OpenTelemetry;

public static class ServicesExtensions
{
    public static void AddAspNetTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var telemetryConfig = configuration.GetSection("OpenTelemetry");
        var serviceName = telemetryConfig.GetValue<string>("ServiceName")
                          ?? throw new ArgumentException("Telemetry service name is missing");
        var endpoint = telemetryConfig.GetValue<string>("ExporterUrl")
                       ?? throw new ArgumentException("Telemetry exporter endpoint is missing");
        services.AddOpenTelemetry()
            .ConfigureResource(resource => 
                resource.AddService(serviceName: serviceName))
            .UseOtlpExporter(OtlpExportProtocol.Grpc, new Uri(endpoint))
            .WithTracing(tracing => tracing
                .AddHttpClientInstrumentation(options =>
                {
                    options.EnrichWithHttpResponseMessage = (activity, httpResponseMessage) =>
                    {
                        if (httpResponseMessage?.Content == null) return;
                        var responseBody = httpResponseMessage.Content.ReadAsStringAsync().Result;  
                        activity.SetTag("http.response.body", responseBody);
                    };  
                })
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.EnrichWithHttpResponse = (activity, httpResponse) =>
                    {
                        if (httpResponse?.Body == null || !httpResponse!.Body.CanRead) return;
                        using var reader = new StreamReader(httpResponse.Body);  
                        var responseBody = reader.ReadToEndAsync().Result;  
                        activity.SetTag("http.response.body", responseBody);  
                        httpResponse.Body.Position = 0;
                    };  
                }))
            .WithLogging()
            .WithMetrics(metrics => metrics
                .AddHttpClientInstrumentation()  
                .AddAspNetCoreInstrumentation());
    }
}