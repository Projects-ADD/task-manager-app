# Test Instructions — Task Manager

## Requisitos previos

- .NET 8 SDK (version `8.0.421`, ver `global.json`)
- Git (opcional, solo para clonar)

## 1. Clonar y compilar

```bash
git clone <repo-url> task-manager
cd task-manager

dotnet restore
dotnet build
```

## 2. Ejecutar todos los tests

```bash
dotnet test
```

Esto corre todos los proyectos de test de la solución. Salida esperada:

```
Test Run Successful.
Total tests: 56
     Passed: 56
```

## 3. Ejecutar solo un proyecto de test

```bash
# Unit tests (dominio y controladores)
dotnet test tests/TaskManager.UnitTests

# Architecture tests (dependencias entre capas)
dotnet test tests/TaskManager.ArchitectureTests
```

## 4. Ejecutar un test específico

```bash
# Por nombre
dotnet test --filter "FullyQualifiedName~PermissionTests"

# Por proyecto + filtro
dotnet test tests/TaskManager.UnitTests --filter "ClassName~RoleTests"
```

## 5. Ejecutar con más detalle

```bash
dotnet test --verbosity detailed
```

## 6. Ejecutar en modo Release (CI)

```bash
dotnet test --configuration Release
```

## 7. Limpiar y re-ejecutar (si hay caché)

```bash
dotnet clean
dotnet test
```

## 8. Ver cobertura de código (opcional)

Requiere `coverlet`:

```bash
dotnet test --collect:"XPlat Code Coverage"
# Generar reporte HTML (requiere reportgenerator)
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

## Estructura de tests

```
tests/
├── TaskManager.UnitTests/
│   ├── Domain/
│   │   ├── BaseEntityTests.cs        # defaults, herencia AggregateRoot
│   │   ├── PermissionTests.cs        # CRUD, colecciones, soft delete
│   │   ├── RoleTests.cs              # CRUD, AssignPermission, duplicados
│   │   └── RolePermissionTests.cs    # creación, ids únicos
│   │
│   └── Controllers/
│       ├── RolesControllerTests.cs      # CRUD, Assign/Revoke/GetPermissions
│       └── PermissionsControllerTests.cs # CRUD completo
│
└── TaskManager.ArchitectureTests/
    └── Tests/
        └── DependencyRulesTests.cs   # 15 reglas hexagonales
```

## Stack de testing

- **xUnit** — framework de tests
- **FluentAssertions** — aserciones legibles
- **Moq** — mocking de servicios para tests de controladores
- **NetArchTest.Rules** — validación de arquitectura hexagonal
- **coverlet** (opcional) — cobertura de código

## Notas

- Los tests no requieren Base de Datos ni Docker.
- Los tests de arquitectura analizan los assemblies compilados, no el código fuente.
- Para agregar un nuevo test de dominio, crear el archivo en `tests/TaskManager.UnitTests/Domain/`.
- Para agregar un nuevo test de controlador, crear el archivo en `tests/TaskManager.UnitTests/Controllers/`.
- Para agregar una nueva regla de arquitectura, editar `tests/TaskManager.ArchitectureTests/Tests/DependencyRulesTests.cs`.
