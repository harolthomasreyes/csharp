# gRPC Bidirectional Streaming - Service A & Service B

## Objetivo

Dos servicios .NET 8 separados que se comunican mediante gRPC con streaming bidireccional usando protobuf.

- **Servicio A (Cliente)**: Solicita una cantidad X de registros al Servicio B
- **Servicio B (Servidor)**: Genera registros aleatorios con estructura JSON y los envía en streaming al Servicio A

## Estructura del Proyecto

```
gRPC/
├── Protos/
│   └── grpc.proto              # Archivo protobuf compartido
├── GrpcServiceA/               # Cliente gRPC
│   ├── GrpcServiceA.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Services/
│   │   └── DataGeneratorClient.cs
│   └── Protos/
│       └── grpc.proto          # Enlace al proto compartido
├── GrpcServiceB/               # Servidor gRPC
│   ├── GrpcServiceB.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Services/
│   │   └── DataGeneratorService.cs
│   └── Protos/
│       └── grpc.proto          # Enlace al proto compartido
└── README.md
```

## Definición Protobuf (grpc.proto)

Ubicación: `Protos/grpc.proto`

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

### Campos de GenerateResponse

| Campo | Tipo | Descripción |
|-------|------|-------------|
| id | int32 | Identificador único del registro |
| name | string | Nombre aleatorio generado |
| value | double | Valor numérico aleatorio |
| timestamp | string | Fecha/hora ISO 8601 del registro |
| json_data | string | JSON serializado con los campos id, name, value, timestamp |

## Pasos de Implementación

### 1. Configurar GrpcServiceB (Servidor)

#### Actualizar `GrpcServiceB.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <Protobuf Include="..\Protos\grpc.proto" GrpcServices="Server" Link="Protos\grpc.proto" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Grpc.AspNetCore" Version="2.59.0" />
  </ItemGroup>
</Project>
```

#### Crear `GrpcServiceB/Services/DataGeneratorService.cs`:

```csharp
using GrpcShared;
using Grpc.Core;
using System.Text.Json;

namespace GrpcServiceB.Services;

public class DataGeneratorService : DataGeneratorService.DataGeneratorServiceBase
{
    private readonly ILogger<DataGeneratorService> _logger;
    private readonly Random _random = new();
    private readonly string[] _names = {
        "Alice", "Bob", "Charlie", "Diana", "Eve",
        "Frank", "Grace", "Hank", "Ivy", "Jack"
    };

    public DataGeneratorService(ILogger<DataGeneratorService> logger)
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

            // Pequeña pausa para simular procesamiento
            await Task.Delay(100, context.CancellationToken);
        }

        _logger.LogInformation("Completado: se enviaron {Count} registros", request.Count);
    }
}
```

#### Actualizar `GrpcServiceB/Program.cs`:

```csharp
using GrpcServiceB.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<DataGeneratorService>();

app.MapGet("/", () => "Communication with gRPC must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
```

#### Actualizar `GrpcServiceB/appsettings.json`:

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
  },
  "Endpoints": {
    "GrpcServiceB": {
      "Url": "http://localhost:5001"
    }
  }
}
```

#### Actualizar `GrpcServiceB/Properties/launchSettings.json`:

```json
{
  "profiles": {
    "GrpcServiceB": {
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

### 2. Configurar GrpcServiceA (Cliente)

#### Actualizar `GrpcServiceA.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <Protobuf Include="..\Protos\grpc.proto" GrpcServices="Client" Link="Protos\grpc.proto" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Grpc.Net.Client" Version="2.59.0" />
    <PackageReference Include="Grpc.AspNetCore" Version="2.59.0" />
  </ItemGroup>
</Project>
```

#### Crear `GrpcServiceA/Services/DataGeneratorClient.cs`:

```csharp
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

    public async Task ReceiveRecordsAsync(string serviceUrl, int count)
    {
        var channel = GrpcChannel.ForAddress(serviceUrl);
        var client = new DataGeneratorService.DataGeneratorServiceClient(channel);

        var responseStream = client.GenerateRecords(new GenerateRequest { Count = count });

        _logger.LogInformation("Conectado a {ServiceUrl}. Esperando {Count} registros...", serviceUrl, count);

        int received = 0;
        await foreach (var response in responseStream.ResponseStream.ReadAllAsync())
        {
            received++;
            _logger.LogInformation("[{Received}/{Total}] Id={Id}, Name={Name}, Value={Value}, Timestamp={Timestamp}",
                received, count, response.Id, response.Name, response.Value, response.Timestamp);

            // Opcional: deserializar JSON
            if (!string.IsNullOrEmpty(response.JsonData))
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, object>>(response.JsonData);
                _logger.LogDebug("JSON recibido: {JsonData}", response.JsonData);
            }
        }

        _logger.LogInformation("Completado: se recibieron {Received} registros de {Total}", received, count);
    }
}
```

#### Actualizar `GrpcServiceA/Program.cs`:

```csharp
using GrpcServiceA.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddSingleton<DataGeneratorClient>();

var app = builder.Build();

// Endpoint HTTP para触发 la solicitud de registros
app.MapPost("/api/records", async (int count, DataGeneratorClient client) =>
{
    var serviceBUrl = app.Configuration["Endpoints:GrpcServiceB:Url"] ?? "http://localhost:5001";
    
    try
    {
        await client.ReceiveRecordsAsync(serviceBUrl, count);
        return Results.Ok(new { message = $"Se generaron y recibieron {count} registros" });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/", () => "API Endpoint: POST /api/records?count=10");

app.Run();
```

#### Actualizar `GrpcServiceA/appsettings.json`:

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

#### Actualizar `GrpcServiceA/Properties/launchSettings.json`:

```json
{
  "profiles": {
    "GrpcServiceA": {
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

## Ejecución

### 1. Restaurar paquetes y compilar

```bash
cd /home/harolpcwork/Documentos/Projects/csharp/gRPC

# Compilar todo
dotnet restore
dotnet build
```

### 2. Iniciar Servicio B (Servidor)

```bash
cd /home/harolpcwork/Documentos/Projects/csharp/gRPC/GrpcServiceB
dotnet run
```

El servicio B escuchará en `http://localhost:5001`

### 3. Iniciar Servicio A (Cliente)

```bash
cd /home/harolpcwork/Documentos/Projects/csharp/gRPC/GrpcServiceA
dotnet run
```

El servicio A escuchará en `http://localhost:5000`

### 4. Solicitar registros (POST)

```bash
# Solicitar 10 registros
curl -X POST "http://localhost:5000/api/records?count=10"

# Solicitar 5 registros
curl -X POST "http://localhost:5000/api/records?count=5"
```

---

## Flujo de Comunicación

```
Servicio A (Cliente)                    Servicio B (Servidor)
      |                                        |
      |  GenerateRequest(count=10)             |
      |---------------------------------------->|
      |                                        |
      |  GenerateResponse(id=1, ...)           |
      |<----------------------------------------|
      |  GenerateResponse(id=2, ...)           |
      |<----------------------------------------|
      |  GenerateResponse(id=3, ...)           |
      |<----------------------------------------|
      |         ...                            |
      |                                        |
      |  GenerateResponse(id=10, ...)          |
      |<----------------------------------------|
      |                                        |
      |  [Streaming completado]                |
```

## Notas Técnicas

1. **Streaming**: Se usa `stream GenerateResponse` en la definición protobuf para enviar registros uno por uno
2. **Target Framework**: Ambos servicios usan `.NET 8` (net8.0)
3. **Proto Files**: El archivo `grpc.proto` está en `Protos/` y se referencia desde ambos proyectos con `<Protobuf Include="..\Protos\grpc.proto" />`
4. **HTTP2**: Kestrel está configurado para usar HTTP2 como protocolo por defecto
5. **CancellationToken**: El streaming respeta el cancellation token para limpieza adecuada
6. **JSON**: Cada respuesta incluye el campo `json_data` con los datos serializados en JSON

## Troubleshooting

### Error: "The target server is not responding"
- Verificar que Servicio B esté corriendo en `http://localhost:5001`
- Verificar que `Endpoints:GrpcServiceB:Url` en appsettings.json apunte al puerto correcto

### Error: "HTTP/2 over HTTP is not supported"
- Asegurarse de que Kestrel tenga `"Protocols": "Http2"` en appsettings.json
- Usar `http://` (no `https://`) para conexiones locales sin SSL

### Error: Proto compilation
- Verificar que el archivo `grpc.proto` exista en `Protos/`
- Verificar que ambos proyectos reference el proto correctamente en el .csproj
