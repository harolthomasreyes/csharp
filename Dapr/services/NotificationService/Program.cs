using NotificationService;
using NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<NotificationProcessor>();

builder.Services.AddDaprClient();

var app = builder.Build();

app.MapDaprSubscriber();

app.MapControllers();

app.Run();
