using Microsoft.EntityFrameworkCore;
using UserApi.Repository;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.Run();