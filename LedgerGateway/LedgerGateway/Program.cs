// using LedgerGateway.RestClients;
using Microsoft.OpenApi.Models;
using ServiceCommons.ApiKey;

var builder = WebApplication.CreateBuilder(args);

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


ConfigureClients(builder);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "LedgerGateway v1"); });
}

app.Run();
return;

void ConfigureClients(WebApplicationBuilder webApplicationBuilder)
{
    var integration = builder.Configuration.GetSection("Integration");
    
    webApplicationBuilder.Services.AddTransient<ApiKeyHttpMesageHandler>(_ =>
    {
        var apiKey = GetOrThrow("ClientApi", "ApiKey");
        return new ApiKeyHttpMesageHandler(apiKey);
    });

    // webApplicationBuilder.Services.AddHttpClient<UserApiClient>(client =>
    // {
    //     client.BaseAddress = new Uri(GetOrThrow("ClientApi", "BaseUrl"));
    // })
    // .AddHttpMessageHandler<ApiKeyHttpMesageHandler>();
    
    webApplicationBuilder.Services.AddTransient<ApiKeyGrpcInterceptor>(_ =>
    {
        var apiKey = GetOrThrow("FinancialService", "ApiKey");
        return new ApiKeyGrpcInterceptor(apiKey);
    });
    
    webApplicationBuilder.Services.AddGrpcClient<FinancialService.FinancialService.FinancialServiceClient>(o =>
    {
        o.Address =new Uri(GetOrThrow("FinancialService", "BaseUrl"));
    }).AddInterceptor<ApiKeyGrpcInterceptor>();
    return;

    string GetOrThrow(string serviceName, string key)
    {
        return integration.GetSection(serviceName).GetValue<string>(key) 
            ?? throw new InvalidOperationException($"{serviceName}.{key} not configured.");
    }
}
