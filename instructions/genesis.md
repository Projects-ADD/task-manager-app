Me gusta mucho el enfoque que estás tomando.

❌✅

La mayoría de tutoriales empiezan por crear controladores y endpoints. Nosotros vamos a hacerlo como se haría en un proyecto profesional:

```plaintext
1. Preparar entorno
2. Crear repositorio
3. Crear solución .NET
4. Crear proyectos
5. Configurar Docker
6. Configurar PostgreSQL
7. Crear "Hello World"
8. Probar API
9. Conectar EF Core
10. Crear primeras entidades
```

No vamos a crear aún Users, Roles o Tasks. Primero debemos asegurarnos de que la arquitectura arranca correctamente.

---

# Fase 0 - Prerrequisitos ✅

Instalar:

## .NET SDK

Versión recomendada actualmente:

```plaintext
.NET 8 LTS
```

Verificar:

```bash
dotnet --version
```

---

## Docker Desktop  ✅

Verificar:

```bash
docker --version
docker compose version
```

---

## VSCode ✅

Extensiones:

```plaintext
C#
PlantUML
Docker
REST Client
```

---

## Git ✅

Verificar:

```bash
git --version
```

---

# Fase 1 - Crear carpeta raíz 

```bash
mkdir task-manager

cd task-manager
```

Inicializar git:

```bash
git init
```

Ejecuta estos comandos dentro de la carpeta raíz del repositorio:

```bash
git config --local user.name "Tu Nombre"
git config --local user.email "tu-correo@ejemplo.com"
```

Para verificar que quedaron configurados solo en este repositorio:
```bash
git config --local --get user.name
git config --local --get user.email
```

Si quieres, también puedes revisar todo lo configurado a nivel local:
```bash
git config --local --list
```


---

# Fase 2 - Crear estructura inicial

```bash
mkdir src
mkdir tests
mkdir docs
mkdir docker
```

Resultado:

```plaintext
📁 task-manager
│
├── 📁 src
├── 📁 tests
├── 📁 docs
└── 📁 docker
```

---

# Fase 3 - Crear solución .NET

Desde la raíz:

```bash
dotnet new sln -n TaskManager
```

Generará:

```plaintext
📄 TaskManager.sln
```

---

# Fase 4 - Crear proyectos

Entrar a src:

```bash
cd src
```

---

## API

```bash
dotnet new webapi -n TaskManager.Api
```

---

## Application

```bash
dotnet new classlib -n TaskManager.Application
```

---

## Domain

```bash
dotnet new classlib -n TaskManager.Domain
```

---

## Infrastructure

```bash
dotnet new classlib -n TaskManager.Infrastructure
```

---

## Contracts

```bash
dotnet new classlib -n TaskManager.Contracts
```

---

Volver a la raíz:

```bash
cd ..
```

---

# Fase 5 - Agregar proyectos a la solución

```bash
dotnet sln add src/TaskManager.Api/TaskManager.Api.csproj

dotnet sln add src/TaskManager.Application/TaskManager.Application.csproj

dotnet sln add src/TaskManager.Domain/TaskManager.Domain.csproj

dotnet sln add src/TaskManager.Infrastructure/TaskManager.Infrastructure.csproj

dotnet sln add src/TaskManager.Contracts/TaskManager.Contracts.csproj
```

---

# Fase 6 - Referencias entre proyectos

---

## Application depende de Domain

```bash
dotnet add src/TaskManager.Application/TaskManager.Application.csproj reference src/TaskManager.Domain/TaskManager.Domain.csproj
```

---

## Infrastructure depende de Domain

```bash
dotnet add src/TaskManager.Infrastructure/TaskManager.Infrastructure.csproj reference src/TaskManager.Domain/TaskManager.Domain.csproj
```

---

## API depende de todos

```bash
dotnet add src/TaskManager.Api/TaskManager.Api.csproj reference src/TaskManager.Application/TaskManager.Application.csproj

dotnet add src/TaskManager.Api/TaskManager.Api.csproj reference src/TaskManager.Infrastructure/TaskManager.Infrastructure.csproj

dotnet add src/TaskManager.Api/TaskManager.Api.csproj reference src/TaskManager.Contracts/TaskManager.Contracts.csproj
```

---

# Fase 7 - Crear el primer endpoint

Abrir:

```plaintext
src/
└── TaskManager.Api/
    └── Program.cs
```

Reemplazar por:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI();

app.MapGet("/", () =>
{
    return Results.Ok("Task Manager API running");
});

app.Run();
```

---

# Fase 8 - Ejecutar

Desde la raíz:

```bash
dotnet run --project src/TaskManager.Api
```

Abrir:

```plaintext
http://localhost:5000
```

o

```plaintext
https://localhost:7000
```

Respuesta:

```json
"Task Manager API running"
```

---

# Fase 9 - Crear Docker para la API

Dentro de:

```plaintext
docker/
```

crear:

```plaintext
📄 Dockerfile.api
```

Contenido:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY . .

RUN dotnet publish src/TaskManager.Api/TaskManager.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TaskManager.Api.dll"]
```

---

# Fase 10 - Crear PostgreSQL

Crear:

```plaintext
docker/docker-compose.yml
```

Contenido:

```yaml
services:

  postgres:

    image: postgres:17

    container_name: task-manager-db

    restart: always

    environment:
      POSTGRES_DB: task_manager
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres

    ports:
      - "5432:5432"

    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

---

Levantar BD:

```bash
docker compose -f docker/docker-compose.yml up -d
```

Verificar:

```bash
docker ps
```

---

# Estado esperado al terminar

```plaintext
📁 task-manager
│
├── 📁 docker
│   ├── 📄 Dockerfile.api
│   └── 📄 docker-compose.yml
│
├── 📁 docs
│
├── 📁 tests
│
├── 📁 src
│   │
│   ├── 📁 TaskManager.Api
│   ├── 📁 TaskManager.Application
│   ├── 📁 TaskManager.Domain
│   ├── 📁 TaskManager.Infrastructure
│   └── 📁 TaskManager.Contracts
│
└── 📄 TaskManager.sln
```

---

## ¿Qué sigue después?

Una vez que tengas este "Hello World" funcionando:

1. Crear la estructura interna de `Domain`.
2. Crear `BaseEntity`.
3. Crear las entidades (`User`, `Role`, `Permission`, `Task`, `TaskAssignment`).
4. Instalar Entity Framework Core.
5. Crear `TaskManagerDbContext`.
6. Generar la primera migración.
7. Levantar PostgreSQL desde Docker.
8. Aplicar migraciones automáticamente.

Ese sería el siguiente bloque de trabajo serio antes de escribir el primer CRUD.
