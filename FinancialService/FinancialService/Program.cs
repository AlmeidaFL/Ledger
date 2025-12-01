using FinancialService.Application;
using FinancialService.Application.Services;
using FinancialService.Messaging;
using FinancialService.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KafkaUserCreatedConsumerSettings>(
    builder.Configuration.GetSection("Kafka:UserCreatedConsumer"));

builder.Services.AddHostedService<UserCreatedConsumerWorker>();
builder.Services.AddScoped<IUserCreatedHandler, UserCreatedHandler>();
builder.Services.AddScoped<IDepositService, DepositService>();
builder.Services.AddScoped<ITransferService, TransferService>();
builder.Services.AddScoped<IAccountLockService, AccountLockService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FinancialDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});


// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();


app.MapGrpcService<FinancialService.Grpc.FinancialService>();

app.Run();