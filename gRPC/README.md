# gRPC Data Generator - Servicio A & Servicio B

## Visión General

Dos servicios .NET 10 independientes que se comunican mediante **gRPC con streaming** usando **protobuf**.

- **Servicio A** (puerto 5000): Cliente con interfaz Swagger UI. Recibe una petición HTTP con la cantidad de registros y la solicita al Servicio B.
- **Servicio B** (puerto 5001): Servidor gRPC. Genera registros aleatorios y los envía en streaming al Servicio A.

## Arquitectura

```
┌─────────────────────────────────────────────────────────────┐
│                     Usuario / Navegador                      │
│                    Swagger UI: /swagger                      │
└──────────────────────────┬──────────────────────────────────┘
                           │ POST /api/records?count=10
                           ▼
┌─────────────────────────────────────────────────────────────┐
│                 Servicio A (puerto 5000)                     │
│  ┌──────────────┐    ┌──────────────────┐                   │
│  │ Swagger UI   │    │ DataGenerator    │                   │
│  │ (Interfaz)   │    │ Client           │                   │
│  └──────────────┘    └────────┬─────────┘                   │
│                               │ gRPC Streaming               │
└───────────────────────────────┼─────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────┐
│                 Servicio B (puerto 5001)                     │
│  ┌──────────────────────────────────────────────┐           │
│  │ GeneratorService                              │           │
│  │ - Genera registros random                     │           │
│  │ - Envía en streaming                          │           │
│  └──────────────────────────────────────────────┘           │
└─────────────────────────────────────────────────────────────┘
```

## Flujo de Comunicación

1. Usuario abre Swagger UI en `http://localhost:5000/swagger`
2. Usuario envía `POST /api/records?count=10`
3. Servicio A conecta al Servicio B vía gRPC
4. Servicio B genera registros random (id, name, value, timestamp, json_data)
5. Servicio B envía cada registro en streaming al Servicio A
6. Servicio A recibe los registros y los devuelve como respuesta HTTP

## Cómo Usar

### 1. Iniciar Servicio B (Servidor gRPC)

```bash
cd /home/harolpcwork/Documentos/Projects/csharp/gRPC/GrpcServiceB
dotnet run
```

### 2. Iniciar Servicio A (Cliente + Swagger)

```bash
cd /home/harolpcwork/Documentos/Projects/csharp/gRPC/GrpcServiceA
dotnet run
```

### 3. Abrir Swagger UI

```
http://localhost:5000/swagger
```

### 4. Enviar Petición

En Swagger UI, expande `POST /api/records`, ingresa `count` (ej: 10) y ejecuta.

O con curl:

```bash
curl -X POST "http://localhost:5000/api/records?count=10"
```

## Registros Generados

Cada registro contiene:

| Campo | Tipo | Descripción |
|-------|------|-------------|
| id | int32 | Identificador único |
| name | string | Nombre aleatorio |
| value | double | Valor numérico aleatorio |
| timestamp | string | Fecha/hora ISO 8601 |
| json_data | string | JSON serializado con los campos anteriores |

## Puertos

| Servicio | Puerto |
|----------|--------|
| Servicio A | 5000 |
| Servicio B | 5001 |

## Tecnologías

- **.NET 10** (net10.0)
- **gRPC** con streaming
- **Protobuf** (proto3)
- **Swagger UI** (Swashbuckle)
- **HTTP/2** (Kestrel)

## Para Detalles Técnicos

Consulta `TECHNICAL.md` para definiciones técnicas completas: código de cada archivo, configuración del proto, troubleshooting, etc.
