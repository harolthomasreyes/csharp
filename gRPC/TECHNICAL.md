# Definiciones Técnicas - gRPC Data Generator

## Estructura de Archivos

```
gRPC/
├── Protos/
│   └── grpc.proto
├── GrpcServiceA/
│   ├── GrpcServiceA.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Properties/launchSettings.json
│   └── Services/
│       ├── DataGeneratorClient.cs
│       └── RecordService.cs
├── GrpcServiceB/
│   ├── GrpcServiceB.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Properties/launchSettings.json
│   └── Services/
│       └── GeneratorService.cs
├── README.md
└── TECHNICAL.md
```

---

## 1. Protobuf Shared (grpc.proto)

**Ubicación:** `Protos/grpc.proto`

```protobuf
syntax = "proto3";

option csharp_namespace = "GrpcShared";

package grpcstreaming;

service DataGeneratorService {
  rpc GenerateRecords (GenerateRequest) returns (stream GenerateResponse) {}
}

message GenerateRequest {
  int32 count = 1;
}

message GenerateResponse {
  int32 id = 1;
  string name = 2;
  double value = 3;
  string timestamp = 4;
  string json_data = 5;
}
```

---

## 2. GrpcServiceB (Servidor)

### GrpcServiceB.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <Protobuf Include="..\Protos\grpc.proto" GrpcServices="Server" Link="Protos\grpc.proto" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Grpc.AspNetCore" Version="2.64.0" />
  </ItemGroup>

</Project>
```

### Services/GeneratorService.cs

```csharp
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
```

### Program.cs

```csharp
using GrpcServiceB.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<GeneratorService>();

app.MapGet("/", () => "Communication with gRPC must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
```

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Kestrel": {
    "EndpointDefaults": {
      "Protocols": "Http2"
    }
  }
}
```

### Properties/launchSettings.json

```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5001",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

---

## 3. GrpcServiceA (Cliente + Swagger)

### GrpcServiceA.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <Protobuf Include="..\Protos\grpc.proto" GrpcServices="Client" Link="Protos\grpc.proto" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Grpc.AspNetCore" Version="2.64.0" />
    <PackageReference Include="Grpc.Net.Client" Version="2.67.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="7.0.0" />
  </ItemGroup>

</Project>
```

### Services/DataGeneratorClient.cs

```csharp
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

        _logger.LogInformation("Conectado a {ServiceUrl}. Esperando {Count} registros...", serviceUrl, count);

        var results = new List<GenerateResponse>();

        await foreach (var response in call.ResponseStream.ReadAllAsync())
        {
            results.Add(response);
            _logger.LogInformation("[{Received}/{Total}] Id={Id}, Name={Name}, Value={Value}, Timestamp={Timestamp}",
                results.Count, count, response.Id, response.Name, response.Value, response.Timestamp);

            if (!string.IsNullOrEmpty(response.JsonData))
            {
                _logger.LogDebug("JSON recibido: {JsonData}", response.JsonData);
            }
        }

        _logger.LogInformation("Completado: se recibieron {Received} registros de {Total}", results.Count, count);

        return results;
    }
}
```

### Services/RecordService.cs

```csharp
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
            _logger.LogInformation("Endpoint completado: {Count} registros recibidos", records.Count);
            return Results.Ok(records);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al recibir registros");
            return Results.Problem(ex.Message);
        }
    }
}
```

### Program.cs

```csharp
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
```

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Endpoints": {
    "GrpcServiceB": {
      "Url": "http://localhost:5001"
    }
  }
}
```

### Properties/launchSettings.json

```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

---

## 4. Compilación

```bash
# Servicio B
cd /home/harolpcwork/Documentos/Projects/csharp/gRPC/GrpcServiceB
dotnet build

# Servicio A
cd /home/harolpcwork/Documentos/Projects/csharp/gRPC/GrpcServiceA
dotnet build
```

## 5. Ejecución

```bash
# Terminal 1 - Servidor
cd /home/harolpcwork/Documentos/Projects/csharp/gRPC/GrpcServiceB
dotnet run

# Terminal 2 - Cliente
cd /home/harolpcwork/Documentos/Projects/csharp/gRPC/GrpcServiceA
dotnet run
```

## 6. Troubleshooting

### Error: "The target server is not responding"
- Verificar que Servicio B esté corriendo en `http://localhost:5001`
- Verificar que `Endpoints:GrpcServiceB:Url` en appsettings.json apunte al puerto correcto

### Error: "HTTP/2 over HTTP is not supported"
- Asegurarse de que Kestrel tenga `"Protocols": "Http2"` en appsettings.json
- Usar `http://` (no `https://`) para conexiones locales sin SSL

### Error: Proto compilation
- Verificar que el archivo `grpc.proto` exista en `Protos/`
- Verificar que ambos proyectos reference el proto correctamente en el .csproj

### Error: Namespace circular
- No nombrar la clase C# igual que el servicio protobuf
- Se usó `GeneratorService` en lugar de `DataGeneratorService` para evitar conflicto
