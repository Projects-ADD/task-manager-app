# AGENTS.md — Task Manager

## Arquitectura

Monolito Modular + Hexagonal (Ports & Adapters). 5 proyectos en una solución:

| Proyecto | Rol | Depende de |
|----------|-----|------------|
| `TaskManager.Api` | Entrypoint ASP.NET, Controllers, Middleware | Application, Infrastructure, Contracts |
| `TaskManager.Application` | Casos de uso, DTOs, validadores, interfaces de servicio | Domain |
| `TaskManager.Domain` | Entidades, ValueObjects, Enums, eventos de dominio | Ninguno |
| `TaskManager.Infrastructure` | EF Core, PostgreSQL, repositorios, JWT | Domain, Application |
| `TaskManager.Contracts` | DTOs públicos Request/Response | Ninguno |

**Reglas:**
- Domain no tiene dependencias de paquetes externos (sin EF, sin ASP.NET).
- Application nunca referencia Infrastructure directamente — solo depende de interfaces definidas en `Application/Common/Interfaces/`.
- Infrastructure referencia Application (para esas interfaces).
- Contracts son DTOs puros; no referencian otros proyectos.

## Stack tecnológico

- .NET 8 (SDK `8.0.421`, `rollForward: latestFeature`), ver `global.json`
- PostgreSQL 17 via Docker
- EF Core 8.0.27 + Npgsql 8.0.11
- Borrado suave en todas las entidades (`DeletedAt`, `IsActive`)
- `ImplicitUsings` + `Nullable` habilitados en todos los proyectos

## Estado actual

### Flujos implementados (Controller → Service → Repository → DB)

| Entidad | CRUD | Endpoints adicionales | Estado |
|---------|------|-----------------------|--------|
| `Permission` | Completo | — | ✅ |
| `Role` | Completo | `POST /api/roles/{roleId}/permissions/{permissionId}` (asignar permiso) | ✅ |
| `User` | — | — | 🔴 Stub |
| `Task` | — | — | 🔴 Stub |
| `TaskAssignment` | — | — | 🔴 Stub |

### Entidades de dominio

- `Permission` — `Name`, `Description`, `RolePermissions` (collection nav). Métodos: `Update()`, `Delete()`.
- `Role` — `Name`, `Description`, `RolePermissions` (collection nav). Métodos: `Update()`, `Delete()`, `AssignPermission(Guid permissionId)`.
- `RolePermission` — Join entity con `RoleId`, `PermissionId`, unique index `(RoleId, PermissionId)`. Creada por `Role.AssignPermission()`. **Nueva**.
- `User` — `Email`, `PasswordHash`, `Roles` (collection). Stub.
- `Task` (colisiona con `System.Threading.Tasks.Task`) — `Title`, `Description`, `Status`, `DueAt`. Stub.
- `TaskAssignment` — `TaskId`, `UserId`, `AssignmentRole`. Stub.

### Capas por proyecto

| Proyecto | Archivos clave |
|----------|---------------|
| `TaskManager.Api/Controllers/` | `PermissionsController.cs` (CRUD), `RolesController.cs` (CRUD + AssignPermission) |
| `TaskManager.Application/Features/Permissions/` | `PermissionService.cs`, `IPermissionService.cs`, `DTOs/PermissionDto.cs` |
| `TaskManager.Application/Features/Roles/` | `RoleService.cs`, `IRoleService.cs`, `DTOs/RoleDto.cs` |
| `TaskManager.Application/Common/Interfaces/` | `IPermissionRepository.cs`, `IRoleRepository.cs` |
| `TaskManager.Infrastructure/Repositories/` | `PermissionRepository.cs`, `RoleRepository.cs` |
| `TaskManager.Infrastructure/Persistence/Configurations/` | `PermissionConfiguration.cs`, `RoleConfiguration.cs`, `RolePermissionConfiguration.cs` |
| `TaskManager.Contracts/Requests/` | `CreatePermissionRequest`, `UpdatePermissionRequest`, `CreateRoleRequest`, `UpdateRoleRequest` |
| `TaskManager.Contracts/Responses/` | `PermissionResponse`, `RoleResponse` |

### Pendiente / no implementado

- `DomainEvent.cs` en `Domain/Common/` — archivo vacío, sin conectar.
- Directorios vacíos en Domain: `ValueObjects/`, `Events/`, `Exceptions/`.
- No hay proyectos de test poblados (directorio `tests/` vacío).
- Sin CI/CD, sin linter/formatter más allá de `.editorconfig`.
- `instructions/` está en `.gitignore` — contiene diario de desarrollo/errores, no configuración autoritativa.
- `Class1.cs` (template por defecto) sigue presente en `TaskManager.Domain` y `TaskManager.Contracts` — eliminar.

## Comandos clave

```bash
# Ejecutar API
dotnet run --project src/TaskManager.Api

# Compilar toda la solución
dotnet build

# Ejecutar tests (no existen aún — placeholder)
dotnet test

# Iniciar PostgreSQL
docker compose -f docker/docker-compose.yml up -d

# Migraciones EF Core (ejecutar desde raíz de solución)
dotnet ef migrations add <Nombre> --project src/TaskManager.Infrastructure
dotnet ef database update --project src/TaskManager.Infrastructure

# Publicar para Docker
dotnet publish src/TaskManager.Api/TaskManager.Api.csproj -c Release -o /app/publish

# Docker build multi-stage
docker build -f docker/Dockerfile.api -t task-manager-api .
```

## Patrones de dominio

- Todas las entidades heredan de `AggregateRoot : BaseEntity` (`Id`, `CreatedAt`, `DeletedAt`, `DeletedBy`, `IsActive`).
- `TaskAssignment` y `RolePermission` heredan de `BaseEntity` directamente (no son aggregate roots).
- Constructores privados + setters privados para materialización de EF Core.
- Borrado suave: `Delete()` asigna `DeletedAt = DateTime.UtcNow`; las consultas filtran `DeletedAt == null`.
- **Problema conocido:** `Permission.Delete()` no asigna `DeletedBy` (ver `docs/Todo.md`).
- La clase `Domain.Entities.Task` colisiona con `System.Threading.Tasks.Task` — usar nombre fully qualified o alias `using TaskAsync = System.Threading.Tasks.Task` cuando sea necesario.

## Peculiaridades de repositorios

- `IPermissionRepository` y `IRoleRepository` usan el alias `using TaskAsync = System.Threading.Tasks.Task` para evitar colisión con la entidad `Task` de Domain.
- Al agregar nuevos repositorios, seguir la misma convención de alias si la entidad `Task` está involucrada.
- `RoleRepository.GetByIdWithPermissionsAsync` incluye `RolePermissions` navigation property (`.Include(r => r.RolePermissions)`).
- Ambos repositorios filtran `DeletedAt == null` en consultas de lista/obtención.

## Configuración de desarrollo

- Conexión PostgreSQL: `Host=localhost;Port=5432;Database=task_manager;Username=postgres;Password=postgres`
- Iniciar BD antes de ejecutar API: `docker compose -f docker/docker-compose.yml up -d`
- API escucha en puertos Kestrel por defecto (5000/7000) si no se sobreescribe en `Properties/launchSettings.json`
- Migraciones EF Core ya existen: `InitialCreate` en `Infrastructure/Migrations/` (crea tablas `Permissions`, `Roles`, `RolePermissions` con FKs e índice único compuesto).

## Convenciones existentes

- `.editorconfig`: indentación 4 espacios para `.cs`, 2 espacios para `.json`/`.yml`/`.yaml`, saltos de línea LF.
- `.gitignore` ignora `bin/`, `obj/`, `.vs/`, `instructions/`, archivos de secretos, volúmenes Docker, artefactos de cobertura.
- `appsettings.Development.json` usa el nombre de BD `taskmanager` (nota: difiere de `appsettings.json` que usa `task_manager`).

## Problemas conocidos

1. `Permission.Delete()` no asigna `DeletedBy` — solo setea `DeletedAt`.
2. **Concurrencia en `RoleService.AssignPermissionAsync`:** EF Core intenta hacer UPDATE en vez de INSERT al agregar un `RolePermission` a la colección navegable de `Role`, lo que causa `DbUpdateConcurrencyException`. Ver `instructions/current_error.txt` para detalle.
3. Nombres de BD inconsistentes entre `appsettings.json` (`task_manager`) y `appsettings.Development.json` (`taskmanager`).
4. `Class1.cs` (default template) presente en `TaskManager.Domain` y `TaskManager.Contracts` sin uso.
