using GrpcShared;
using Grpc.Core;
using System.Text.Json;

namespace GrpcServiceB.Services;

public class GeneratorService : DataGeneratorService.DataGeneratorServiceBase
{
    private readonly ILogger<GeneratorService> _logger;
    private readonly Random _random = new();
    private readonly string[] _names = {
        "Alice", "Bob", "Charlie", "Diana", "Eve",
        "Frank", "Grace", "Hank", "Ivy", "Jack"
    };

    public GeneratorService(ILogger<GeneratorService> logger)
    {
        _logger = logger;
    }

    public override async Task GenerateRecords(GenerateRequest request, IServerStreamWriter<GenerateResponse> responseStream, ServerCallContext context)
    {
        _logger.LogInformation("Solicitud recibida: generar {Count} registros", request.Count);

        for (int i = 0; i < request.Count; i++)
        {
            var name = _names[_random.Next(_names.Length)];
            var value = Math.Round(_random.NextDouble() * 1000, 2);
            var timestamp = DateTime.UtcNow.ToString("O");

            var jsonData = JsonSerializer.Serialize(new
            {
                id = i + 1,
                name,
                value,
                timestamp
            });

            var response = new GenerateResponse
            {
                Id = i + 1,
                Name = name,
                Value = value,
                Timestamp = timestamp,
                JsonData = jsonData
            };

            await responseStream.WriteAsync(response);

            _logger.LogInformation("Enviado registro {Id}: {Name} = {Value}", response.Id, response.Name, response.Value);

            await Task.Delay(100, context.CancellationToken);
        }

        _logger.LogInformation("Completado: se enviaron {Count} registros", request.Count);
    }
}
