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
