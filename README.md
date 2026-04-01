# Store Services App

Aplicacion de microservicios para la gestion de una tienda de libros, desarrollada con .NET 10 y desplegable con Docker Compose.

## Arquitectura

El proyecto sigue una arquitectura de microservicios con el patron CQRS usando MediatR:

| Servicio | Descripcion | Base de datos | Puerto |
|---|---|---|---|
| **Author API** | Gestion de autores | PostgreSQL 17.5 | 6104 |
| **Book API** | Catalogo de libros | SQL Server (Azure SQL Edge) | 6105 |
| **ShoppingCart API** | Carrito de compras | MariaDB 11 | 6106 |
| **Gateway** | API Gateway con Ocelot | - | 6107 |

## Tecnologias

- .NET 10 (Minimal API)
- Entity Framework Core 10
- MediatR 14
- AutoMapper
- FluentValidation
- Ocelot (API Gateway)
- xUnit + Moq (testing)
- Docker / Docker Compose

## Estructura del proyecto

```
store-services-app/
├── src/
│   ├── StoreServices.Api.Author/          # Microservicio de autores
│   │   ├── Application/                   # Handlers, DTOs, validaciones
│   │   ├── Controllers/                   # Endpoints HTTP
│   │   ├── Model/                         # Entidades
│   │   └── Persistence/                   # DbContext
│   ├── StoreServices.Api.Book/            # Microservicio de libros
│   ├── StoreServices.Api.ShoppingCart/    # Microservicio de carrito
│   │   ├── RemoteInterface/               # Interfaz del servicio remoto
│   │   ├── RemoteModel/                   # Modelos remotos
│   │   └── RemoteService/                 # Cliente HTTP hacia Book API
│   └── StoreServices.Api.Gateway/         # API Gateway (Ocelot)
├── test/
│   ├── StoreServices.Api.Author.Tests/    # Tests unitarios de Author
│   ├── StoreServices.Api.Book.Tests/      # Tests unitarios de Book
│   └── StoreServices.Api.ShoppingCart.Tests/  # Tests unitarios de ShoppingCart
└── .deploy/                               # Docker Compose y configuracion
```

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## Inicio rapido

### Con Docker Compose

```bash
docker compose -f .deploy/docker-compose.yml -f .deploy/docker-compose.override.yml up -d
```

Esto levanta los 4 microservicios junto con PostgreSQL, SQL Server y MariaDB.

### Desarrollo local

```bash
# Restaurar dependencias
dotnet restore

# Compilar
dotnet build

# Ejecutar tests
dotnet test
```

## Endpoints

### Author API (puerto 6104)

| Metodo | Ruta | Descripcion |
|---|---|---|
| GET | `/api/Author` | Listar autores |
| GET | `/api/Author/{id}` | Obtener autor por GUID |
| POST | `/api/Author` | Crear autor |

### Book API (puerto 6105)

| Metodo | Ruta | Descripcion |
|---|---|---|
| GET | `/api/BookMaterial` | Listar libros |
| GET | `/api/BookMaterial/{id}` | Obtener libro por ID |
| POST | `/api/BookMaterial` | Crear libro |

### ShoppingCart API (puerto 6106)

| Metodo | Ruta | Descripcion |
|---|---|---|
| GET | `/api/ShoppingCart/{id}` | Obtener carrito por ID |
| POST | `/api/ShoppingCart` | Crear carrito |

### Gateway (puerto 6107)

| Ruta Gateway | Redirige a |
|---|---|
| `/Book` | Book API - `GET`, `POST`, `PUT` |
| `/Book/{id}` | Book API - `GET`, `DELETE` |

## Tests

61 tests unitarios organizados con el patron AAA (Arrange, Act, Assert):

```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar tests de un proyecto especifico
dotnet test test/StoreServices.Api.Book.Tests

# Ejecutar con detalle
dotnet test --verbosity normal
```

## Docker

```bash
# Construir imagenes
docker compose -f .deploy/docker-compose.yml -f .deploy/docker-compose.override.yml build

# Levantar servicios
docker compose -f .deploy/docker-compose.yml -f .deploy/docker-compose.override.yml up -d

# Ver logs
docker compose -f .deploy/docker-compose.yml -f .deploy/docker-compose.override.yml logs -f

# Parar servicios
docker compose -f .deploy/docker-compose.yml -f .deploy/docker-compose.override.yml down
```
