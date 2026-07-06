# Task Manager API

API REST para gestión de tareas, construida con **.NET 8** bajo una arquitectura **Hexagonal (Ports & Adapters)** como **Monolito Modular**. Diseñada para escalar hacia un sistema tipo Trello/Jira, con énfasis en principios de diseño limpios, separación de responsabilidades y capacidad de evolución.

---

## Arquitectura

```mermaid
flowchart LR

    Client[Web UI / Mobile UI]

    API[TaskManager.Api]

    APP[Application Layer]

    DOMAIN[Domain Layer]

    INFRA[Infrastructure Layer]

    DB[(PostgreSQL)]

    Client --> API

    API --> APP

    API --> INFRA

    APP --> DOMAIN

    INFRA --> APP

    INFRA --> DB
```

### Proyectos

| Proyecto | Responsabilidad | Dependencias |
|----------|----------------|--------------|
| `TaskManager.Api` | Entrypoint HTTP, controladores, middleware | Application, Infrastructure, Contracts |
| `TaskManager.Application` | Casos de uso, servicios de aplicación, DTOs | Domain |
| `TaskManager.Domain` | Entidades, value objects, enums, eventos de dominio | Ninguna |
| `TaskManager.Infrastructure` | Persistencia EF Core, PostgreSQL, repositorios, JWT | Domain, Application |
| `TaskManager.Contracts` | Contratos públicos de request/response | Ninguna |

### Reglas arquitectónicas

- **Domain** no tiene dependencias externas (sin EF Core, sin ASP.NET, sin paquetes NuGet).
- **Application** nunca referencia a Infrastructure directamente. Se comunica mediante interfaces definidas en `Application/Common/Interfaces/`.
- **Infrastructure** implementa esas interfaces y referencia a Application.
- **Contracts** son DTOs planos; no referencian otros proyectos de la solución.

---

## Stack tecnológico

| Componente | Versión |
|------------|---------|
| .NET SDK | `8.0.421` (`rollForward: latestFeature`) |
| ASP.NET Core | 8.0 |
| Entity Framework Core | `8.0.27` |
| Npgsql (EF Core Provider) | `8.0.11` |
| PostgreSQL | 17 |
| Docker | 24+ (con Docker Compose v2) |

---

## Prerrequisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (con Docker Compose v2)
- Git

Verificar instalación:

```bash
dotnet --version
docker --version
docker compose version
```

---

## Inicio rápido (desarrollo)

### 1. Clonar el repositorio

```bash
git clone <url-del-repositorio>
cd task-manager-app
```

### 2. Iniciar PostgreSQL

```bash
docker compose -f docker/docker-compose.yml up -d postgres
```

Esto levanta PostgreSQL 17 en `localhost:5432`, base de datos `taskmanager`, usuario `postgres`, contraseña `postgres`.

### 3. Aplicar migraciones

```bash
dotnet ef database update --project src/TaskManager.Infrastructure --startup-project src/TaskManager.Api
```

### 4. Ejecutar la API

```bash
dotnet run --project src/TaskManager.Api
```

La API se inicia en:
- HTTP: `http://localhost:5225`
- HTTPS: `https://localhost:7100`

### 5. Verificar

```bash
curl http://localhost:5225
# → "Task Manager API running"
```

O abrir `http://localhost:5225` en el navegador.

---

## Comandos disponibles

```bash
# Limpiar la solución completa
dotnet clean

# Restaurar paquetes NuGet (Opcional, se hace automáticamente al compilar)
dotnet restore

# Compilar la solución completa
dotnet build

# Ejecutar la API
dotnet run --project src/TaskManager.Api

# Ejecutar tests (cuando existan)
dotnet test

# Publicar para producción
dotnet publish src/TaskManager.Api/TaskManager.Api.csproj \
    -c Release \
    -o ./publish

# Crear una migración de EF Core
dotnet ef migrations add <NombreMigracion> \
    --project src/TaskManager.Infrastructure \
    --startup-project src/TaskManager.Api

# Aplicar migraciones a la BD
dotnet ef database update \
    --project src/TaskManager.Infrastructure \
    --startup-project src/TaskManager.Api

# Iniciar sólo PostgreSQL
docker compose -f docker/docker-compose.yml up -d postgres

# Iniciar sólo la API (requiere BD externa)
docker compose -f docker/docker-compose.yml up -d api

# Iniciar API + PostgreSQL (todo con Docker)
docker compose -f docker/docker-compose.yml up -d

# Detener servicios (juntos)
docker compose -f docker/docker-compose.yml down

# Detener servicios (uno por uno)
docker compose -f docker/docker-compose.yml stop <servicio>

#Detener servicios y eliminar contenedores, redes y volúmenes
docker compose -f docker/docker-compose.yml down -v 

# Detener servicios y eliminar contenedores, redes y volúmenes (uno por uno)
docker compose -f docker/docker-compose.yml down -v <servicio>

# Reiniciar servicios (uno por uno)
docker compose -f docker/docker-compose.yml restart <servicio>



# Ver logs
docker compose -f docker/docker-compose.yml logs -f
```

---

## Endpoints disponibles

### Permissions (`/api/permissions`)

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/api/permissions` | Crear un permiso |
| `GET` | `/api/permissions` | Listar todos los permisos activos |
| `GET` | `/api/permissions/{id:guid}` | Obtener permiso por ID |
| `PUT` | `/api/permissions/{id:guid}` | Actualizar permiso |
| `DELETE` | `/api/permissions/{id:guid}` | Eliminación lógica de permiso |

```bash
curl -X POST http://localhost:5000/api/permissions \
  -H "Content-Type: application/json" \
  -d '{"name": "task:create", "description": "Crear tareas"}'
```

### Roles (`/api/roles`)

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/api/roles` | Crear un rol |
| `GET` | `/api/roles` | Listar todos los roles activos |
| `GET` | `/api/roles/{id:guid}` | Obtener rol por ID |
| `PUT` | `/api/roles/{id:guid}` | Actualizar rol |
| `DELETE` | `/api/roles/{id:guid}` | Eliminación lógica de rol |

```bash
curl -X POST http://localhost:5000/api/roles \
  -H "Content-Type: application/json" \
  -d '{"name": "Administrador", "description": "Acceso total al sistema"}'
```

---

## Estructura del proyecto

```
task-manager-app/
├── src/
│   ├── TaskManager.Api/          # Controladores, Program.cs, configuración
│   ├── TaskManager.Application/  # Casos de uso, servicios, DTOs
│   ├── TaskManager.Domain/       # Entidades, value objects, enums
│   ├── TaskManager.Infrastructure/ # EF Core, repositorios, migraciones
│   └── TaskManager.Contracts/    # Request/Response DTOs públicos
├── docker/
│   ├── docker-compose.yml        # PostgreSQL 17 + API
│   └── Dockerfile.api            # Build de producción para la API
├── tests/                        # Proyectos de test (pendiente)
├── docs/                         # Documentación y diagramas
├── .editorconfig                 # Convenciones de formato
├── .gitignore
├── global.json                   # Fijación de versión de SDK
├── TaskManager.sln
├── LICENSE
└── README.md
```

---

## Despliegue

### Docker (API + PostgreSQL)

Levanta ambos servicios (base de datos y API) con un solo comando:

```bash
docker compose -f docker/docker-compose.yml up -d
```

La API queda disponible en `http://localhost:5000` y PostgreSQL en `localhost:5432`.

Para aplicar las migraciones automáticamente al iniciar el contenedor, ejecuta:

```bash
dotnet ef database update \
    --project src/TaskManager.Infrastructure \
    --startup-project src/TaskManager.Api
```

> Si la base de datos es externa (Neon.tech, OCI, etc.), sobrescribe la variable `ConnectionStrings__DefaultConnection` al iniciar el contenedor.

### Construir la imagen manualmente

```bash
docker build -f docker/Dockerfile.api -t task-manager-api .
```

### Ejecutar solo la API (conectada a una BD externa)

```bash
docker run -d \
  --name task-manager-api \
  -p 5000:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="Host=<host>;Port=5432;Database=taskmanager;Username=<user>;Password=<password>" \
  task-manager-api
```

### Publicación directa (sin Docker)

```bash
dotnet publish src/TaskManager.Api/TaskManager.Api.csproj \
    -c Release \
    -o /app/publish
cd /app/publish
dotnet TaskManager.Api.dll
```

---

### Despliegue en Oracle OCI (Oracle Cloud Infrastructure)

1. **Crea una instancia de VM** (Oracle Linux o Ubuntu) con acceso a puerto 5000.
2. **Instala Docker** en la VM:
   ```bash
   sudo dnf install -y docker
   sudo systemctl enable --now docker
   ```
3. **Sube la imagen** a un registro (Oracle Container Registry, Docker Hub, o cárgala directo):
   ```bash
   docker build -f docker/Dockerfile.api -t task-manager-api .
   docker tag task-manager-api <region>.ocir.io/<namespace>/task-manager-api:latest
   docker push <region>.ocir.io/<namespace>/task-manager-api:latest
   ```
4. **En la VM, ejecuta el contenedor** apuntando a tu base de datos PostgreSQL (puede ser OCI MySQL/PostgreSQL o externa):
   ```bash
   docker run -d \
     --name task-manager-api \
     -p 5000:8080 \
     -e ASPNETCORE_ENVIRONMENT=Production \
     -e ConnectionStrings__DefaultConnection="Host=<oci-db-host>;Port=5432;Database=taskmanager;Username=<user>;Password=<password>" \
     <region>.ocir.io/<namespace>/task-manager-api:latest
   ```
5. **Aplica migraciones** (ejecutar dentro del contenedor o desde tu máquina si la BD es accesible):
   ```bash
   dotnet ef database update \
     --project src/TaskManager.Infrastructure \
     --startup-project src/TaskManager.Api
   ```

### Despliegue con Neon.tech (PostgreSQL serverless) + contenedor local/cloud

1. **Crea una base de datos gratuita** en [Neon.tech](https://neon.tech).
2. **Obtén la cadena de conexión** (parecida a `Host=ep-xxx.us-east-2.aws.neon.tech;...;sslmode=require`).
3. **Ejecuta el contenedor de la API apuntando a Neon**:
   ```bash
   docker run -d \
     --name task-manager-api \
     -p 5000:8080 \
     -e ASPNETCORE_ENVIRONMENT=Production \
     -e ConnectionStrings__DefaultConnection="Host=<neon-host>;Port=5432;Database=taskmanager;Username=<user>;Password=<password>;sslmode=require" \
     task-manager-api
   ```
4. **Aplica migraciones** desde tu máquina local (necesitas la cadena de Neon configurada en `appsettings.json` o vía variable):
   ```bash
   dotnet ef database update \
     --project src/TaskManager.Infrastructure \
     --startup-project src/TaskManager.Api
   ```

> ⚠️ **Importante**: en producción, no uses la cadena de conexión directamente en variables de entorno. Usa un administrador de secretos (OCI Vault, AWS Secrets Manager, o variables de entorno cifradas).

---

### Despliegue en Render.com

Render soporta despliegue desde Docker o desde un archivo `render.yaml` (Blueprints).

**Opción A — Desde el dashboard (manual):**

1. Conecta tu repositorio de GitHub a Render.
2. Crea primero una base de datos **PostgreSQL** desde el dashboard de Render.
3. Espera a que la instancia quede aprovisionada y copia su **External Database URL** o la cadena de conexión equivalente.
4. Crea un **Web Service** y selecciona el mismo repositorio.
5. En el tipo de despliegue, elige **Docker** para que Render construya la imagen desde el repositorio.
6. Configura el servicio con estos valores mínimos:
  - **Branch**: `main`
  - **Dockerfile Path**: `docker/Dockerfile.api`
  - **Docker Context**: repositorio raíz (`.`)
  - **Health Check Path**: `/`
7. Define estas variables de entorno en el Web Service:
  - `ASPNETCORE_ENVIRONMENT=Production`
  - `ASPNETCORE_URLS=http://+:8080`
  - `ConnectionStrings__DefaultConnection=<cadena-del-PostgreSQL-de-Render>`
8. Crea el Web Service y espera a que termine el primer build.
9. Aplica las migraciones contra la base de datos de Render desde tu máquina local:
  ```bash
  dotnet ef database update \
    --project src/TaskManager.Infrastructure \
    --startup-project src/TaskManager.Api
  ```
10. Verifica el despliegue abriendo la URL pública del servicio o haciendo una petición a `/`.

> En esta opción manual no debes usar `runtime: image` ni proporcionar una imagen preconstruida. Aquí Render construye el contenedor desde `docker/Dockerfile.api`.

**Opción B — Blueprint (`render.yaml`):**

Ya existe un archivo [`render.yaml`](render.yaml) en la raíz del proyecto que define:

- **Web Service** `task-manager-api` — usa `runtime: docker`, construye desde `docker/Dockerfile.api` y expone puerto `8080`.
- **PostgreSQL** `taskmanager-db` — base de datos administrada por Render (plan free).

Solo tienes que:

1. Sustituir `repo: https://github.com/<tu-usuario>/<tu-repo>` en `render.yaml` con tu repositorio real.
2. Conectar tu repo a Render.
3. Ir a **Dashboard → Blueprint** y seleccionar el repo.
4. Render detecta el `render.yaml` y crea automáticamente la base de datos + el servicio web, inyectando la variable `ConnectionStrings__DefaultConnection` con la cadena correcta.

> Render tarda ~2-3 minutos en aprovisionar la BD gratuita. Después del primer deploy, las migraciones deben aplicarse manualmente (Render no ejecuta `dotnet ef database update` automáticamente).

---

## Estado del proyecto

El proyecto se encuentra en fase de construcción activa:

- ✅ Arquitectura fundamental definida y funcional
- ✅ Flujo CRUD completo para `Permission`
- ✅ Flujo CRUD completo para `Role`
- ✅ Flujo CRUD completo para `Task`
- 🔄 `User`, `TaskAssignment` — entidades definidas, lógica de negocio pendiente
- ⏳ Eventos de dominio — por conectar
- ⏳ Tests unitarios, de integración y de arquitectura — por implementar
- ⏳ Autenticación JWT — por implementar
- ⏳ CI/CD — por configurar

Consulta [`docs/Todo.md`](docs/Todo.md) para ver la lista de pendientes conocidos.

---

## Licencia

Distribuido bajo licencia MIT. Ver [`LICENSE`](LICENSE).

---

## Autor

**Adrian Ortega** — Proyecto de portafolio profesional de arquitectura de software.
