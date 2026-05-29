using Grpc.Core;
using GrpcShared;
using Grpc.Net.Client;
using System.Text.Json;

namespace GrpcServiceA.Services;

public class DataGeneratorClient
{
    private readonly ILogger<DataGeneratorClient> _logger;

    public DataGeneratorClient(ILogger<DataGeneratorClient> logger)
    {
        _logger = logger;
    }

    public async Task<List<GenerateResponse>> ReceiveRecordsAsync(string serviceUrl, int count)
    {
        var channel = GrpcChannel.ForAddress(serviceUrl);
        var client = new DataGeneratorService.DataGeneratorServiceClient(channel);

        var call = client.GenerateRecords(new GenerateRequest { Count = count });

        //_logger.LogInformation("Conectado a {ServiceUrl}. Esperando {Count} registros...", serviceUrl, count);

        var results = new List<GenerateResponse>();

        await foreach (var response in call.ResponseStream.ReadAllAsync())
        {
            results.Add(response);
            //_logger.LogInformation("[{Received}/{Total}] Id={Id}, Name={Name}, Value={Value}, Timestamp={Timestamp}",results.Count, count, response.Id, response.Name, response.Value, response.Timestamp);

            if (!string.IsNullOrEmpty(response.JsonData))
            {
                //_logger.LogDebug("JSON recibido: {JsonData}", response.JsonData);
            }
        }

        _logger.LogInformation("Completado: se recibieron {Received} registros de {Total}", results.Count, count);

        return results;
    }
}
