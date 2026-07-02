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