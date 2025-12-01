using Microsoft.OpenApi.Models;

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

builder.Services.AddGrpcClient<FinancialService.FinancialService.FinancialServiceClient>(o =>
{
    o.Address = new Uri(builder.Configuration["FinancialService:GrpcUrl"]
                        ?? throw new InvalidOperationException("Missing FinancialService:GrpcUrl"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "LedgerGateway v1"); });
}


app.Run();
