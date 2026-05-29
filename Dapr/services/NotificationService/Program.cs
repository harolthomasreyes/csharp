using NotificationService;
using NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<NotificationProcessor>();

builder.Services.AddDaprClient();

var app = builder.Build();

// Habilitar Swagger UI siempre
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NotificationService API V1");
});

app.MapControllers();

app.Run();
