using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UserApi.Repository;
using UserApi.Services;
using ServiceCommons.ApiKey;
using UserApi.Application.Handlers;
using UserApi.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KafkaFinancialAccountCreatedSettings>(
    builder.Configuration.GetSection("Kafka:FinancialAccountCreatedConsumer"));

builder.Services.AddInternalApiKeyAuthentication(builder.Configuration);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IFinancialAccountCreatedHandler, FinancialAccountCreatedHandler>();
builder.Services.AddHostedService<FinancialAccountCreatedWorker>();

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

var app = builder.Build();

// app.UseAuthentication();
// app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserApi v1");
    });
}

app.MapControllers();
app.Run();