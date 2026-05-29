using Microsoft.EntityFrameworkCore;
using OrderService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
{
    var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
    var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
    var db = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "dapr_outbox";
    var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "dapr";
    var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "dapr_secret";
    
    options.UseNpgsql($"Host={host};Port={port};Database={db};Username={user};Password={password}");
});

builder.Services.AddDaprClient();

var app = builder.Build();

// Habilitar Swagger UI siempre
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderService API V1");
});

app.MapControllers();

app.Run();
