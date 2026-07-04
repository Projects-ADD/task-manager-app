# Todo

## Pendientes

- [ ] Implementar la relación `DeletedBy` para indicar qué usuario borró un registro.
    Primero reflejarlo en el diagrama y después implementarlo en código.

- [ ] Completar el uso funcional de `DeletedBy`.
    El tipo ya está definido, pero en la implementación actual de `Permission.Delete()` todavía no se asigna valor.

- [ ] Evaluar e implementar un campo `updates` con historial de actualizaciones por entidad de dominio.
    Revisar bien el diseño antes de incorporarlo.

## Completados

- [x] Alinear en el diagrama `.mmd` los tipos de PK y FK con el esquema base definido por `BaseEntity.cs`.

- [x] Indicar en el diagrama `.mmd` que `DeletedAt` es nullable.

## Notas

- `DeletedBy` ya tiene tipo correcto en el diagrama y en el modelo base, pero su comportamiento aún no está implementado completamente.


## En proceso de implementar la relación entre `Role` y `Permission` (muchos a muchos) según el diagrama [data-model.mmd].

De acuerdo al diagrama [data-model.mmd]  debe haber una tabla intermedia para hacer la relación ¿qué pasos siguen para poder crearla?

Además de eso, hay que hacer la lógica de asignación/remoción de permisos al rol.
	- Agregar permisos al rol
		- ya sea que se mande una lista de permisos
		- se mande 1 permiso a la vez
	- Remover permisos al rol
		- lista de ids de permisos asociados al rol

Funciones complementarias:
	- Listar los permisos asignados a un rol (id)
	- Listar los roles, cada rol con su lista de permisos (debería haber un parámetro tipo ?show_permissions=true) que deje decidir mostrar los permisos
	- Si se elimina el rol, desacoplar los permisos asociados a esta


---

# TODO - Task Manager API

## Estado actual

### RBAC Base

* [x] CRUD de Roles
* [x] CRUD de Permisos
* [x] Tabla intermedia `RolePermissions`
* [x] Asignar un permiso individual a un rol

  * Endpoint:

    * `POST /roles/{roleId}/permissions/{permissionId}`

---

# Fase 1 - Gestión básica de permisos por rol

## Remover permiso individual de un rol

* [x] Implementar lógica de dominio para remover permisos asociados a un rol.
* [x] Crear endpoint:

  * `DELETE /roles/{roleId}/permissions/{permissionId}`
* [x] Validar que solamente se elimine la relación y no el permiso.
* [ ] Agregar pruebas manuales en Postman.

## Obtener permisos asociados a un rol

* [x] Crear endpoint:

  * `GET /roles/{roleId}/permissions`
* [x] Retornar lista de permisos asociados al rol.
* [x] Definir DTO de respuesta.
* [x] Validar comportamiento cuando el rol no existe.


[✓] Add GetPermissionsByRoleAsync to IRoleService interface
[✓] Implement GetPermissionsByRoleAsync in RoleService
[✓] Update RoleRepository to include Permission navigation in GetByIdWithPermissionsAsync
[✓] Add GET /roles/{roleId}/permissions endpoint in RolesController
[✓] Build and verify the project compiles

---

# Fase 2 - Operaciones masivas

## Asignación masiva de permisos

* [ ] Crear endpoint:

  * `POST /roles/{roleId}/permissions`
* [ ] Recibir lista de `permissionIds`.
* [ ] Evitar duplicados.
* [ ] Validar permisos inexistentes.

Ejemplo:

```json
{
  "permissionIds": [
    "uuid-1",
    "uuid-2",
    "uuid-3"
  ]
}
```

## Remoción masiva de permisos

* [ ] Crear endpoint:

  * `DELETE /roles/{roleId}/permissions`
* [ ] Recibir lista de permisos a eliminar.
* [ ] Eliminar únicamente las relaciones existentes.

Ejemplo:

```json
{
  "permissionIds": [
    "uuid-1",
    "uuid-2"
  ]
}
```

---

# Fase 3 - Consultas enriquecidas

## Obtener roles con permisos incluidos

* [ ] Implementar parámetro opcional:

  * `GET /roles?includePermissions=true`
* [ ] Mantener compatibilidad con:

  * `GET /roles`
* [ ] Diseñar DTO específico para la respuesta enriquecida.

## Obtener detalle de rol con permisos

* [ ] Crear endpoint:

  * `GET /roles/{id}`
* [ ] Incluir permisos asociados opcionalmente.
* [ ] Definir parámetro:

  * `includePermissions=true`

---

# Fase 4 - Gestión de usuarios

## Entidad User

* [ ] Crear entidad `User`.
* [ ] Crear configuración EF Core.
* [ ] Crear migración.
* [ ] Implementar CRUD de usuarios.

## Relación User-Roles

* [ ] Crear tabla intermedia `UserRoles`.
* [ ] Implementar asignación de roles a usuarios.
* [ ] Implementar remoción de roles.
* [ ] Implementar listado de roles por usuario.

## Endpoints esperados

* [ ] `POST /users/{userId}/roles/{roleId}`
* [ ] `DELETE /users/{userId}/roles/{roleId}`
* [ ] `GET /users/{userId}/roles`

---

# Fase 5 - Autenticación

## JWT Authentication

* [ ] Implementar login.
* [ ] Generar JWT.
* [ ] Configurar expiración del token.
* [ ] Configurar firma y validación.

## Endpoints

* [ ] `POST /auth/login`
* [ ] `POST /auth/refresh`

---

# Fase 6 - Autorización

## Role Based Access Control

* [ ] Implementar políticas de autorización.
* [ ] Validar permisos desde JWT.
* [ ] Crear atributos personalizados de autorización.

Ejemplos:

```csharp
[RequirePermission("task.create")]
```

```csharp
[RequirePermission("user.delete")]
```

---

# Fase 7 - Auditoría

## Auditoría básica

* [ ] Registrar `CreatedAt`.
* [ ] Registrar `UpdatedAt`.
* [ ] Registrar `DeletedAt`.
* [ ] Registrar `CreatedBy`.
* [ ] Registrar `UpdatedBy`.
* [ ] Registrar `DeletedBy`.

---

# Fase 8 - Calidad y arquitectura

## Validaciones

* [ ] Integrar FluentValidation.
* [ ] Centralizar validaciones.

## Manejo de errores

* [ ] Middleware global de excepciones.
* [ ] Problemas RFC7807 (`ProblemDetails`).

## Logging

* [ ] Logs estructurados.
* [ ] Correlation ID.
* [ ] Request ID.

## Observabilidad

* [ ] Health Checks.
* [ ] Métricas.
* [ ] Tracing distribuido.

---

# Fase 9 - Testing

## Unit Testing

* [ ] Servicios de aplicación.
* [ ] Dominio.
* [ ] Reglas de negocio.

## Integration Testing

* [ ] Endpoints.
* [ ] Base de datos.
* [ ] Autenticación.

## Test Containers

* [ ] PostgreSQL efímero para pruebas automáticas.

---

# Fase 10 - DevOps y despliegue

## Contenedorización

* [ ] Dockerfile para API.
* [ ] Docker Compose para entorno local.

## CI/CD

* [ ] Pipeline de build.
* [ ] Ejecución automática de tests.
* [ ] Publicación automática.

## Infraestructura

* [ ] Variables de entorno.
* [ ] Configuración por ambiente.
* [ ] Secrets Management.

---

# Fase 11 - Dominio del Task Manager

## Gestión de proyectos

* [ ] Crear entidad Project.
* [ ] CRUD de proyectos.

## Gestión de tareas

* [ ] Crear entidad Task.
* [ ] CRUD de tareas.

## Asignaciones

* [ ] Asignar usuarios a tareas.
* [ ] Asignar responsables.

## Estados

* [ ] Backlog
* [ ] Todo
* [ ] In Progress
* [ ] Review
* [ ] Done

## Prioridades

* [ ] Low
* [ ] Medium
* [ ] High
* [ ] Critical

## Comentarios

* [ ] Comentarios en tareas.
* [ ] Historial de cambios.

## Adjuntos

* [ ] Subida de archivos.
* [ ] Gestión de evidencias.

---

# Fase 12 - Características avanzadas

* [ ] Notificaciones.
* [ ] Background Jobs.
* [ ] Event Driven Architecture.
* [ ] Outbox Pattern.
* [ ] Cache distribuido.
* [ ] Rate Limiting.
* [ ] Feature Flags.
* [ ] Multi-tenancy.
* [ ] API Versioning.
* [ ] OpenTelemetry.
* [ ] OpenAPI avanzada.
* [ ] Webhooks.

---

# Hitos sugeridos del portafolio

## v0.5.0

* RBAC funcional.

## v0.6.0

* Usuarios y autenticación.

## v0.7.0

* Autorización completa.

## v0.8.0

* Auditoría y observabilidad.

## v1.0.0

* Primer MVP completo del Task Manager.
