# Dapr Outbox Pattern - .NET 8 + Pulsar + PostgreSQL

Demostracion del patron Outbox de Dapr con .NET 8, Apache Pulsar y PostgreSQL, todo orquestado en contenedores Docker.

## Topologia

```
┌─────────────────────────────────────────────────────────┐
│                    Docker Network                       │
│                                                         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │ OrderService │  │ Notification │  │   Dapr       │  │
│  │  (.NET 8)    │  │   Service    │  │   Sidecar    │  │
│  │  :5000       │  │   (.NET 8)   │  │   :5002      │  │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘  │
│         │                 │                   │         │
│  ┌──────▼───────┐  ┌─────▼────────┐  ┌─────▼───────┐  │
│  │  PostgreSQL  │  │   Pulsar     │  │  Dapr        │  │
│  │  :5432       │  │   :6650      │  │   Dashboard  │  │
│  │              │  │   :8080      │  │   :8081      │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
```

## Requisitos

- Docker Engine 24+
- Docker Compose v2
- .NET 8 SDK (para pruebas locales)
- Git

## Estructura de archivos

```
Dapr/
├── dapr-compose.yml              # Orquestacion Docker (7 contenedores)
├── components/
│   ├── pulsar-broker.yaml        # Dapr pub/sub component (Pulsar)
│   ├── postgres-outbox.yaml      # Dapr outbox component (PostgreSQL)
│   └── dapr-config.yaml          # Configuracion Dapr (tracing)
├── services/
│   ├── OrderService/             # Productor (.NET 8)
│   │   ├── OrderService.csproj
│   │   ├── Program.cs
│   │   ├── Controllers/OrdersController.cs
│   │   ├── Models/Order.cs
│   │   ├── Services/OrderDbContext.cs
│   │   └── Dockerfile
│   └── NotificationService/      # Consumidor (.NET 8)
│       ├── NotificationService.csproj
│       ├── Program.cs
│       ├── NotificationController.cs
│       ├── Services/NotificationProcessor.cs
│       └── Dockerfile
├── init.sql                      # Tablas iniciales de PostgreSQL
└── .env                          # Variables de configuracion
```

## Contenedores

| # | Container | Puerto | Proposito |
|---|-----------|--------|-----------|
| 1 | `order-service` | 5000 | .NET 8 API - productora (Outbox) |
| 2 | `notification-service` | 5001 | .NET 8 API - consumidora |
| 3 | `postgres` | 5432 | Base de datos para Outbox |
| 4 | `pulsar` | 6650, 8080 | Broker de mensajes |
| 5 | `dapr-order` | 5002 | Sidecar Dapr para OrderService |
| 6 | `dapr-notification` | 5004 | Sidecar Dapr para NotificationService |
| 7 | `dapr-dashboard` | 8081 | Dashboard de Dapr |

## Inicio rapido

### 1. Clonar y configurar

```bash
cd Dapr
```

### 2. Iniciar todos los servicios

```bash
docker compose -f dapr-compose.yml up -d
```

### 3. Verificar que todo esta corriendo

```bash
docker compose -f dapr-compose.yml ps
```

Deberias ver los 7 contenedores en estado `healthy`.

### 4. Ver logs

```bash
# Todos los servicios
docker compose -f dapr-compose.yml logs -f

# Solo order-service
docker compose -f dapr-compose.yml logs -f order-service

# Solo notification-service
docker compose -f dapr-compose.yml logs -f notification-service
```

## Flujo del Outbox Pattern

### 1. Crear una orden (productor)

```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "Juan Perez",
    "totalAmount": 150.00
  }'
```

### 2. Verificar la orden en PostgreSQL

```bash
docker exec -it postgres psql -U dapr -d dapr_outbox -c "SELECT * FROM orders;"
```

### 3. Verificar el evento en la tabla outbox

```bash
docker exec -it postgres psql -U dapr -d dapr_outbox -c "SELECT * FROM dapr_outbox;"
```

### 4. Verificar logs del notification-service

```bash
docker compose -f dapr-compose.yml logs notification-service
```

Deberias ver el log:

```
Processing notification: Order <id> created by Juan Perez - Total: $150.00
```

### 5. Verificar topic de Pulsar

```bash
curl http://localhost:8080/admin/v2/persistent/public/default/order-events/partitions
```

## Verificacion del patron Outbox

El patron Outbox garantiza que la orden y el evento se guardan en la misma transaccion:

1. **OrderService** inserta la orden en `orders` Y el evento en `dapr_outbox` dentro de la misma transaccion
2. **Dapr Outbox component** escanea la tabla `dapr_outbox` y publica eventos a Pulsar
3. **NotificationService** recibe el evento via Dapr pub/sub desde Pulsar
4. Si la transaccion falla, el evento NO se publica (atomicidad garantizada)

## Dashboard de Dapr

Accede al dashboard para monitorear los sidecars, componentes y pub/sub:

```
http://localhost:8081
```

## Detener y limpiar

### Detener servicios (sin perder datos)

```bash
docker compose -f dapr-compose.yml stop
```

### Detener y eliminar contenedores (mantener datos)

```bash
docker compose -f dapr-compose.yml down
```

### Eliminar todo (incluyendo datos)

```bash
docker compose -f dapr-compose.yml down -v
```

## Pruebas locales (sin Docker)

### 1. Instalar Dapr CLI

```bash
# Linux
curl -sL https://raw.githubusercontent.com/dapr/cli/master/install/install.sh | bash

# Verificar instalacion
dapr --version
```

### 2. Iniciar Dapr

```bash
dapr init
```

### 3. Iniciar PostgreSQL localmente

```bash
docker run -d --name postgres -e POSTGRES_DB=dapr_outbox -e POSTGRES_USER=dapr -e POSTGRES_PASSWORD=dapr_secret -p 5432:5432 postgres:16-alpine
```

### 4. Iniciar Pulsar localmente

```bash
docker run -d --name pulsar -p 6650:6650 -p 8080:8080 apachepulsar/pulsar:3.3.0 bin/pulsar standalone run
```

### 5. Copiar componentes Dapr

```bash
mkdir -p ~/.dapr/components
cp components/pulsar-broker.yaml ~/.dapr/components/
cp components/postgres-outbox.yaml ~/.dapr/components/
```

### 6. Ejecutar servicios localmente

```bash
# Terminal 1 - OrderService
cd services/OrderService
export POSTGRES_HOST=localhost
export POSTGRES_PORT=5432
export POSTGRES_DB=dapr_outbox
export POSTGRES_USER=dapr
export POSTGRES_PASSWORD=dapr_secret
dapr run --app-id order-service --app-port 8080 -- dotnet run

# Terminal 2 - NotificationService
cd services/NotificationService
export POSTGRES_HOST=localhost
export POSTGRES_PORT=5432
export POSTGRES_DB=dapr_outbox
export POSTGRES_USER=dapr
export POSTGRES_PASSWORD=dapr_secret
dapr run --app-id notification-service --app-port 8080 -- dotnet run
```

### 7. Probar con curl

```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "Test Local",
    "totalAmount": 99.99
  }'
```

## Variables de entorno

| Variable | Valor por defecto | Descripcion |
|----------|-------------------|-------------|
| `POSTGRES_HOST` | `postgres` | Host de PostgreSQL |
| `POSTGRES_PORT` | `5432` | Puerto de PostgreSQL |
| `POSTGRES_DB` | `dapr_outbox` | Nombre de la base de datos |
| `POSTGRES_USER` | `dapr` | Usuario de PostgreSQL |
| `POSTGRES_PASSWORD` | `dapr_secret` | Password de PostgreSQL |
| `PULSAR_BROKER` | `pulsar:6650` | Broker de Pulsar |

## Solucion de problemas

### Contenedor no inicia

```bash
# Ver logs del contenedor
docker compose -f dapr-compose.yml logs <nombre-contenedor>

# Reiniciar un contenedor especifico
docker compose -f dapr-compose.yml restart <nombre-contenedor>
```

### PostgreSQL no acepta conexiones

```bash
# Esperar a que PostgreSQL este listo
docker exec -it postgres pg_isready -U dapr

# Verificar que las tablas existen
docker exec -it postgres psql -U dapr -d dapr_outbox -c "\dt"
```

### Pulsar no responde

```bash
# Verificar que Pulsar esta corriendo
curl http://localhost:8080/admin/v2/standalone/clusters

# Reiniciar Pulsar
docker compose -f dapr-compose.yml restart pulsar
```

### Sidecar Dapr no encuentra componentes

```bash
# Verificar que los componentes estan montados
docker exec -it dapr-order ls /components/

# Verificar logs del sidecar
docker compose -f dapr-compose.yml logs dapr-order
```

### Error de transaccion en Outbox

```bash
# Verificar la tabla outbox
docker exec -it postgres psql -U dapr -d dapr_outbox -c "SELECT * FROM dapr_outbox ORDER BY created_at DESC LIMIT 10;"

# Verificar que las tablas existen
docker exec -it postgres psql -U dapr -d dapr_outbox -c "\dt"
```

## Referencias

- [Dapr Outbox Component](https://docs.dapr.io/reference/components/reference-state-components/postgres/)
- [Dapr Pulsar Pub/Sub](https://docs.dapr.io/reference/components/reference-pubsub/pubsub-pulsar/)
- [Dapr Outbox Pattern](https://docs.dapr.io/developing-applications/building-blocks/outbox/)
- [Apache Pulsar](https://pulsar.apache.org/)
- [.NET 8](https://dotnet.microsoft.com/)
