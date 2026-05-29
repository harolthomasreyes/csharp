using GrpcServiceA.Services;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddSingleton<DataGeneratorClient>();
builder.Services.AddSingleton<RecordService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "gRPC Data Generator API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "gRPC Data Generator API v1");
        c.RoutePrefix = "swagger";
    });
}

app.MapGet("/", () => "Swagger UI: /swagger | API Endpoint: POST /api/records?count=10&serviceUrl=http://localhost:5001");

app.MapPost("/api/records", async (int count, RecordService recordService, IConfiguration config) =>
{
    var serviceUrl = config["Endpoints:GrpcServiceB:Url"] ?? "http://localhost:5001";
    return await recordService.GetRecordsAsync(count, serviceUrl);
});

app.Run();
