using System.Text.Json;
using LedgerGateway;
using LedgerGateway.Application;
using LedgerGateway.RestClients.SimpleAuth;
using LedgerGateway.RestClients.UserApi;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using ServiceCommons.ApiKey;
using ServiceCommons.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddClientAuthentication(builder.Configuration);
builder.Services.AddAspNetTelemetry(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LedgerGateway",
        Version = "v1",
    });
});
builder.Services.AddScoped<ServicesHealthCheck>();
builder.Services.AddSingleton<IUserAgentParser, UserAgentParser>();

ConfigureClients(builder);

builder.Services.AddCors(options =>
{
    options.AddPolicy("gateway-cors", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<ServicesHealthCheck>("external-apis", tags: ["ready"]);

var app = builder.Build();

app.UseCors("gateway-cors");
    
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "LedgerGateway v1"); });
}

app.MapHealthChecks("/health/live", new HealthCheckOptions  
{  
    Predicate = check => check.Tags.Contains("live")  
});  
app.MapHealthChecks("/health/ready", new HealthCheckOptions  
{  
    Predicate = check => check.Tags.Contains("ready")  
});
app.MapHealthChecks("/health", new HealthCheckOptions  
{  
    ResponseWriter = async (context, report) =>  
    {  
        context.Response.ContentType = "application/json";  
        var json = JsonSerializer.Serialize(new   
        {   
            status = report.Status.ToString(), 
            checks = report.Entries.Select(e => new {  
                name = e.Key,  
                status = e.Value.Status.ToString(),  
                description = e.Value.Description  
            })  
        });  
        await context.Response.WriteAsync(json);  
    }  
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
return;

void ConfigureClients(WebApplicationBuilder webApplicationBuilder)
{
    var integration = builder.Configuration.GetSection("Integration");

    webApplicationBuilder.Services.AddHttpClient<SimpleAuthClient>(client =>
    {
        client.BaseAddress = new Uri(GetOrThrow("SimpleAuth", "BaseUrl"));
    })
    .AddHttpMessageHandler(() => new ApiKeyHttpMesageHandler(GetOrThrow("SimpleAuth", "ApiKey")));
    
    webApplicationBuilder.Services.AddHttpClient<UserApiClient>(client =>
    {
        client.BaseAddress = new Uri(GetOrThrow("UserApi", "BaseUrl"));
    })
    .AddHttpMessageHandler(() => new ApiKeyHttpMesageHandler(GetOrThrow("UserApi", "ApiKey")));
    
    webApplicationBuilder.Services.AddGrpcClient<FinancialService.FinancialService.FinancialServiceClient>(o =>
    {
        o.Address = new Uri(GetOrThrow("FinancialService", "BaseUrl"));
    }).AddInterceptor(() => new ApiKeyGrpcInterceptor(GetOrThrow("FinancialService", "ApiKey")));
    return;

    string GetOrThrow(string serviceName, string key)
    {
        return integration.GetSection(serviceName).GetValue<string>(key) 
            ?? throw new InvalidOperationException($"{serviceName}.{key} not configured.");
    }
}
