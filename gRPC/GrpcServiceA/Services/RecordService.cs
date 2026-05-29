using GrpcServiceA.Services;

namespace GrpcServiceA.Services;

public class RecordService
{
    private readonly ILogger<RecordService> _logger;
    private readonly DataGeneratorClient _client;

    public RecordService(ILogger<RecordService> logger, DataGeneratorClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<IResult> GetRecordsAsync(int count, string serviceUrl)
    {
        try
        {
            var records = await _client.ReceiveRecordsAsync(serviceUrl, count);
            //_logger.LogInformation("Endpoint completado: {Count} registros recibidos", records.Count);
            return Results.Ok(records.Count);
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "Error al recibir registros");
            return Results.Problem(ex.Message);
        }
    }
}
