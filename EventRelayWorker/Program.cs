using EventRelayWorker;
using EventRelayWorker.Data;
using EventRelayWorker.Messaging;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<Tenants>(builder.Configuration.GetSection("Tenants"));
builder.Services.Configure<OutboxOptions>(builder.Configuration.GetSection("Outbox"));
builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection("Kafka"));

builder.Services.AddSingleton<OutboxDbContextFactory>();
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.AddHostedService<EventRelayWorker.EventRelayWorker>();

var host = builder.Build();
host.Run();
