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
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation())
            .WithLogging()
            .WithMetrics(metrics => metrics
                .AddHttpClientInstrumentation()  
                .AddAspNetCoreInstrumentation());
    }
}