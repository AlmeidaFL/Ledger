using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using ServiceCommons.ApiKey;
using SimpleAuth.Api.Data;
using SimpleAuth.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInternalApiKeyAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<ILoginAttemptService, LoginAttemptService>();
builder.Services.AddSingleton<IUserAgentParser, UserAgentParser>();
builder.Services.AddScoped<ITemporaryCodeService, TemporaryCodeService>();
builder.Services.AddSingleton<IEmailSender, FakeEmailSender>();
builder.Services.AddScoped<GoogleAuthService>();
builder.Services.AddScoped<ILoginService, LoginService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SimpleAuth",
        Version = "v1",
    });
});

var key = builder.Configuration["Jwt:Key"];
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];
//
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.Events = new JwtBearerEvents()
//         {
//             OnMessageReceived = context =>
//             {
//                 var accessToken = context.Request.Cookies["access_token"];
//                 if (!string.IsNullOrEmpty(accessToken))
//                 {
//                     context.Token = accessToken;
//                 }
//
//                 return Task.CompletedTask;
//             }
//         };
//         
//         options.TokenValidationParameters = new TokenValidationParameters()
//         {
//             ValidateIssuerSigningKey = true,
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
//             ValidateAudience = true,
//             ValidAudience = audience,
//             ValidateIssuer = true,
//             ValidIssuer = issuer,
//             
//             ValidateLifetime = true,
//             ClockSkew = TimeSpan.Zero
//         };
//     });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();