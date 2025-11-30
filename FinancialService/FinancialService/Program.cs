using FinancialService.Application;
using FinancialService.Messaging;
using FinancialService.Repository;
using FinancialService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KafkaUserCreatedConsumerSettings>(
    builder.Configuration.GetSection("Kafka:UserCreatedConsumer"));

builder.Services.AddHostedService<UserCreatedConsumerWorker>();
builder.Services.AddScoped<IUserCreatedHandler, UserCreatedHandler>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FinancialDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});


// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();