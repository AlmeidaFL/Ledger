using EventRelayWorker;
using EventRelayWorker.Data;
using EventRelayWorker.Messaging;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseKestrel();
        webBuilder.ConfigureServices(services =>
        {
            services.AddHealthChecks()
                .AddCheck("worker-ready", () =>
                    WorkerHealth.Ready
                      ? HealthCheckResult.Healthy("Worker ready")
                      : HealthCheckResult.Unhealthy("Worker not ready"));
        });

        webBuilder.Configure(app =>
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health/ready");
            });
        });
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<Tenants>(context.Configuration.GetSection("Tenants"));
        services.Configure<OutboxOptions>(context.Configuration.GetSection("Outbox"));
        services.Configure<KafkaOptions>(context.Configuration.GetSection("Kafka"));
        services.AddSingleton<OutboxDbContextFactory>();
        services.AddSingleton<IKafkaProducer, KafkaProducer>();
        services.AddHostedService<EventRelayWorker.EventRelayWorker>();
    });

var host = builder.Build();
await host.RunAsync();

