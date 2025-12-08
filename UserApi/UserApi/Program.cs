using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using UserApi.Repository;
using UserApi.Services;
using ServiceCommons.ApiKey;
using ServiceCommons.OpenTelemetry;
using UserApi.Messaging;
using UserApi.Messaging.Events;
using UserApi.Messaging.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAspNetTelemetry(builder.Configuration);
builder.Services.Configure<KafkaSettings>(
    builder.Configuration.GetSection("Kafka:FinancialAccountCreatedConsumer"));

builder.Services.AddInternalApiKeyAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddHostedService<KafkaWorker>();

builder.Services.AddScoped<IMessageHandler<UserCreatedEvent>, UserCreatedHandler>();
builder.Services.AddScoped<IMessageHandler<FinancialAccountCreatedEvent>, FinancialAccountCreatedHandler>();
builder.Services.AddScoped(typeof(EventExecution<>));
builder.Services.AddScoped<IKafkaDispatcher, KafkaDispatcher>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UserApi",
        Version = "v1",
    });
});

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddDbContextCheck<UserDbContext>("ready", tags: ["ready"]);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health/live", new HealthCheckOptions  
{  
    Predicate = check => check.Tags.Contains("live")  
});  
app.MapHealthChecks("/health/ready", new HealthCheckOptions  
{  
    Predicate = check => check.Tags.Contains("ready")  
});
app.MapHealthChecks("/health");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();