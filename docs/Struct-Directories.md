# 🏛 Estructura definitiva propuesta

```plaintext
📁 task-manager/
│
├── 📁 docs/
│   ├── 📄 README.md
│   ├── 📄 PRODUCT_REQUIREMENTS.md
│   ├── 📄 ARCHITECTURE.md
│   ├── 📄 DECISIONS.md
│   │
│   ├── 📁 diagrams/
│   │   ├── 📄 task-manager-erd.puml
│   │   ├── 📄 task-manager-domain.puml
│   │   ├── 📄 hexagonal-architecture.puml
│   │   ├── 📄 create-task-sequence.puml
│   │   └── 📄 assign-task-sequence.puml
│   │
│   └── 📁 api/
│       └── 📄 openapi-spec.yaml
│
├── 📁 docker/
│   ├── 📄 docker-compose.yml
│   ├── 📄 Dockerfile.api
│   └── 📄 Dockerfile.db
│
├── 📁 scripts/
│   ├── 📄 create-db.sql
│   ├── 📄 seed-data.sql
│   └── 📄 init-dev-env.sh
│
├── 📁 src/
│
│   ├── 📁 TaskManager.Api/
│   │   ├── 📄 Program.cs
│   │   ├── 📄 appsettings.json
│   │   ├── 📄 appsettings.Development.json
│   │   │
│   │   ├── 📁 Controllers/
│   │   ├── 📁 Middlewares/
│   │   ├── 📁 Filters/
│   │   ├── 📁 Swagger/
│   │   └── 📁 Extensions/
│   │
│   │
│   ├── 📁 TaskManager.Application/
│   │
│   │   ├── 📁 Common/
│   │   │   ├── 📁 Behaviors/
│   │   │   ├── 📁 Exceptions/
│   │   │   ├── 📁 Interfaces/
│   │   │   └── 📁 Models/
│   │   │
│   │   ├── 📁 Features/
│   │   │
│   │   │   ├── 📁 Users/
│   │   │   │   ├── 📁 Commands/
│   │   │   │   ├── 📁 Queries/
│   │   │   │   ├── 📁 DTOs/
│   │   │   │   └── 📁 Validators/
│   │   │   │
│   │   │   ├── 📁 Roles/
│   │   │   ├── 📁 Permissions/
│   │   │   ├── 📁 Tasks/
│   │   │   ├── 📁 Assignments/
│   │   │   ├── 📁 Auth/
│   │   │   ├── 📁 Audit/
│   │   │   └── 📁 DeveloperPortal/
│   │   │
│   │   └── 📄 DependencyInjection.cs
│   │
│   │
│   ├── 📁 TaskManager.Domain/
│   │
│   │   ├── 📁 Common/
│   │   │   ├── 📄 BaseEntity.cs
│   │   │   ├── 📄 DomainEvent.cs
│   │   │   └── 📄 AggregateRoot.cs
│   │   │
│   │   ├── 📁 Entities/
│   │   │   ├── 📄 User.cs
│   │   │   ├── 📄 Role.cs
│   │   │   ├── 📄 Permission.cs
│   │   │   ├── 📄 Task.cs
│   │   │   └── 📄 TaskAssignment.cs
│   │   │
│   │   ├── 📁 Enums/
│   │   │   ├── 📄 TaskStatus.cs
│   │   │   ├── 📄 AssignmentRole.cs
│   │   │   └── 📄 AuditAction.cs
│   │   │
│   │   ├── 📁 ValueObjects/
│   │   │   ├── 📄 Email.cs
│   │   │   └── 📄 PasswordHash.cs
│   │   │
│   │   ├── 📁 Events/
│   │   │   ├── 📄 TaskCreatedEvent.cs
│   │   │   ├── 📄 TaskCompletedEvent.cs
│   │   │   └── 📄 UserCreatedEvent.cs
│   │   │
│   │   └── 📁 Exceptions/
│   │       └── 📄 DomainException.cs
│   │
│   │
│   ├── 📁 TaskManager.Infrastructure/
│   │
│   │   ├── 📁 Persistence/
│   │   │   ├── 📄 TaskManagerDbContext.cs
│   │   │   │
│   │   │   ├── 📁 Configurations/
│   │   │   │   ├── 📄 UserConfiguration.cs
│   │   │   │   ├── 📄 RoleConfiguration.cs
│   │   │   │   ├── 📄 TaskConfiguration.cs
│   │   │   │   └── 📄 TaskAssignmentConfiguration.cs
│   │   │   │
│   │   │   ├── 📁 Migrations/
│   │   │   └── 📁 Seed/
│   │   │
│   │   ├── 📁 Repositories/
│   │   │   ├── 📄 UserRepository.cs
│   │   │   ├── 📄 TaskRepository.cs
│   │   │   └── 📄 RoleRepository.cs
│   │   │
│   │   ├── 📁 Identity/
│   │   │   ├── 📄 JwtProvider.cs
│   │   │   └── 📄 PasswordHasher.cs
│   │   │
│   │   ├── 📁 Services/
│   │   │   ├── 📄 AuditService.cs
│   │   │   └── 📄 DateTimeProvider.cs
│   │   │
│   │   └── 📄 DependencyInjection.cs
│   │
│   │
│   └── 📁 TaskManager.Contracts/
│       │
│       ├── 📁 Requests/
│       ├── 📁 Responses/
│       ├── 📁 Auth/
│       └── 📁 Common/
│       └── 📁 Enums/
│
│
├── 📁 tests/
│
│   ├── 📁 TaskManager.UnitTests/
│   │   ├── 📁 Domain/
│   │   ├── 📁 Application/
│   │   └── 📁 Infrastructure/
│   │
│   ├── 📁 TaskManager.IntegrationTests/
│   │   ├── 📁 API/
│   │   ├── 📁 Database/
│   │   └── 📁 Authentication/
│   │
│   └── 📁 TaskManager.ArchitectureTests/
│       ├── 📄 HexagonalRulesTests.cs
│       └── 📄 DependencyRulesTests.cs
│
├── 📄 .gitignore
├── 📄 .editorconfig
├── 📄 TaskManager.sln
├── 📄 LICENSE
└── 📄 README.md
```

---

# 🎯 Qué representa cada proyecto

## 📁 TaskManager.Api

Contiene:

```plaintext
Controllers
Middlewares
Swagger
```

Es únicamente la puerta de entrada.

No debe contener lógica de negocio.

---

## 📁 TaskManager.Application

Contiene:

```plaintext
Use Cases
Commands
Queries
DTOs
Validators
```

Aquí viven los casos de uso.

Ejemplos:

```plaintext
CreateTask
AssignTask
CompleteTask
CreateUser
Login
```

---

## 📁 TaskManager.Domain

Es el corazón.

Contiene:

```plaintext
User
Role
Permission
Task
TaskAssignment
```

y reglas de negocio.

No conoce:

* PostgreSQL
* JWT
* EF Core
* Controllers

---

## 📁 TaskManager.Infrastructure

Contiene:

```plaintext
EF Core
PostgreSQL
JWT
Repositories
Servicios externos
```

---

## 📁 TaskManager.Contracts

Contiene los contratos públicos.

Ejemplo:

```plaintext
CreateTaskRequest
CreateTaskResponse
LoginRequest
LoginResponse
```

---

