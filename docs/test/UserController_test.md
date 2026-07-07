# UserController - `POST /api/users/{userId}/roles/{roleId}`

## Descripción del endpoint

Asigna un rol existente a un usuario existente.

- **Método:** `POST`
- **URL:** `/api/users/{userId:guid}/roles/{roleId:guid}`
- **Controlador:** `UserController.AssignRole` (`src/TaskManager.Api/Controllers/UserController.cs:447`)
- **Servicio:** `IUserService.AssignRoleAsync` → `UserService.AssignRoleAsync`

### Flujo de ejecución

1. Se reciben `userId` y `roleId` como parámetros de ruta (GUID).
2. Se invoca `_userService.AssignRoleAsync(userId, roleId)`.
3. El servicio busca al usuario por `userId` — si no existe, lanza `NotFoundException`.
4. El servicio busca el rol por `roleId` — si no existe, lanza `NotFoundException`.
5. Se invoca `user.AssignRole(roleId)` en la entidad de dominio — si ya está asignado, lanza `InvalidOperationException`.
6. Se persisten los cambios en base de datos.
7. Se retorna `200 OK` con `ApiResponse`.

> **Nota:** El `try/catch` para `NotFoundException` está comentado (líneas 449-470). Las excepciones no controladas se propagan al middleware de manejo global de errores.

---

## Casos de prueba

### ✅TC-01: Asignación exitosa de un rol a un usuario

| Campo | Valor |
|---|---|
| **ID** | TC-01 |
| **Título** | Asignar rol a usuario existente — respuesta exitosa |
| **Descripción** | Verifica que asignar un rol válido a un usuario válido retorna `200 OK` con el mensaje de éxito. |
| **Precondiciones** | Existe un usuario con `userId`. Existe un rol con `roleId`. El rol **no** está asignado actualmente al usuario. |
| **Datos de entrada** | `userId` = GUID existente, `roleId` = GUID existente |
| **Resultado esperado** | `200 OK` con `ApiResponse { Action = "post", HttpStatusCode = 200, Message = "Role assigned successfully.", Data = null }` |
| **Validaciones adicionales** | La relación `UserRoles` debe persistirse en BD. |

---

### ✅TC-02: Usuario no encontrado (NotFoundException)

| Campo | Valor |
|---|---|
| **ID** | TC-02 |
| **Título** | Asignar rol con userId inexistente |
| **Descripción** | Verifica que al proporcionar un `userId` que no existe en el sistema se retorne `404 Not Found`. |
| **Precondiciones** | No existe un usuario con el `userId` proporcionado. Existe un rol con `roleId`. |
| **Datos de entrada** | `userId` = GUID válido pero inexistente, `roleId` = GUID existente |
| **Resultado esperado** | `404 Not Found` con mensaje `"User with ID {userId} not found."` |
| **Mecanismo** | `UserService.AssignRoleAsync` lanza `NotFoundException`. Actualmente no hay `try/catch` en el controlador, por lo que la excepción se propaga al middleware global. |



❌ hay que validar la estructura del GUID que se le pasa a los endpoints

---

### ✅TC-03: Rol no encontrado (NotFoundException)

| Campo | Valor |
|---|---|
| **ID** | TC-03 |
| **Título** | Asignar rol con roleId inexistente |
| **Descripción** | Verifica que al proporcionar un `roleId` que no existe en el sistema se retorne `404 Not Found`. |
| **Precondiciones** | Existe un usuario con `userId`. No existe un rol con el `roleId` proporcionado. |
| **Datos de entrada** | `userId` = GUID existente, `roleId` = GUID válido pero inexistente |
| **Resultado esperado** | `404 Not Found` con mensaje `"Role with ID {roleId} not found."` |
| **Mecanismo** | `UserService.AssignRoleAsync` lanza `NotFoundException`. Igual que TC-02, se propaga al middleware global. |

---

### ❌ TC-04: Rol ya asignado al usuario (InvalidOperationException)

| Campo | Valor |
|---|---|
| **ID** | TC-04 |
| **Título** | Asignar rol que ya está asignado al usuario |
| **Descripción** | Verifica que al intentar asignar un rol que ya posee el usuario se retorne un error. |
| **Precondiciones** | Existe un usuario con `userId`. Existe un rol con `roleId`. El rol **ya está asignado** al usuario. |
| **Datos de entrada** | `userId` = GUID existente, `roleId` = GUID existente (ya asignado) |
| **Resultado esperado** | `409 Conflict` o `500 Internal Server Error` (dependiendo del manejo global de `InvalidOperationException`). El mensaje debe indicar `"Role is already assigned to the user."`. |
| **Mecanismo** | `user.AssignRole(roleId)` en la entidad de dominio lanza `InvalidOperationException`. Actualmente no hay manejo de esta excepción en el controlador. |

---

### ❌ TC-05: userId con formato inválido (no GUID)

| Campo | Valor |
|---|---|
| **ID** | TC-05 |
| **Título** | userId con formato inválido |
| **Descripción** | Verifica que al proporcionar un `userId` que no es un GUID válido se retorne un error. |
| **Precondiciones** | Ninguna. |
| **Datos de entrada** | `userId` = `"not-a-guid"`, `roleId` = GUID válido |
| **Resultado esperado** | `404 Not Found` (la ruta no coincide debido a la restricción `:guid` en el template `{userId:guid}`). Alternativamente `400 Bad Request` si existe un catch-all o validación adicional. |
| **Mecanismo** | La restricción de ruta `:guid` impide que ASP.NET Core matched la ruta si el valor no es un GUID válido. |

---

### ❌ TC-06: roleId con formato inválido (no GUID)

| Campo | Valor |
|---|---|
| **ID** | TC-06 |
| **Título** | roleId con formato inválido |
| **Descripción** | Verifica que al proporcionar un `roleId` que no es un GUID válido se retorne un error. |
| **Precondiciones** | Ninguna. |
| **Datos de entrada** | `userId` = GUID válido, `roleId` = `"not-a-guid"` |
| **Resultado esperado** | `404 Not Found` (la ruta no coincide debido a la restricción `:guid`). |
| **Mecanismo** | Misma restricción de ruta `:guid` que TC-05. |

---

### ❌ TC-07: Ambos IDs con formato inválido

| Campo | Valor |
|---|---|
| **ID** | TC-07 |
| **Título** | userId y roleId con formato inválido |
| **Descripción** | Verifica que al proporcionar tanto `userId` como `roleId` con formato no GUID se retorne un error. |
| **Precondiciones** | Ninguna. |
| **Datos de entrada** | `userId` = `"invalid"`, `roleId` = `"invalid"` |
| **Resultado esperado** | `404 Not Found` (la ruta no coincide). |
| **Mecanismo** | Misma restricción de ruta `:guid` que TC-05 y TC-06. |

---

### ❌ TC-08: userId es un GUID vacío (all zeros)

| Campo | Valor |
|---|---|
| **ID** | TC-08 |
| **Título** | userId con GUID vacío (00000000-0000-0000-0000-000000000000) |
| **Descripción** | Verifica que al proporcionar un GUID vacío como `userId` (que es un GUID válido sintácticamente pero no corresponde a ningún usuario) se retorne `404 Not Found`. |
| **Precondiciones** | No existe un usuario con `Id = 00000000-0000-0000-0000-000000000000`. |
| **Datos de entrada** | `userId` = `00000000-0000-0000-0000-000000000000`, `roleId` = GUID existente |
| **Resultado esperado** | `404 Not Found` (equivalente a TC-02). El GUID es sintácticamente válido, por lo que la ruta sí coincide, pero el servicio no encuentra al usuario. |

---

### ❌ TC-09: roleId es un GUID vacío (all zeros)

| Campo | Valor |
|---|---|
| **ID** | TC-09 |
| **Título** | roleId con GUID vacío (00000000-0000-0000-0000-000000000000) |
| **Descripción** | Verifica que al proporcionar un GUID vacío como `roleId` se retorne `404 Not Found`. |
| **Precondiciones** | Existe un usuario con `userId`. No existe un rol con `Id = 00000000-0000-0000-0000-000000000000`. |
| **Datos de entrada** | `userId` = GUID existente, `roleId` = `00000000-0000-0000-0000-000000000000` |
| **Resultado esperado** | `404 Not Found` (equivalente a TC-03). |

---

## Resumen de casos

| ID | Escenario | HTTP Status esperado | Mecanismo |
|---|---|---|---|
| TC-01 | Asignación exitosa | `200 OK` | Flujo feliz |
| TC-02 | Usuario no existe | `404 Not Found` | `NotFoundException` en servicio |
| TC-03 | Rol no existe | `404 Not Found` | `NotFoundException` en servicio |
| TC-04 | Rol ya asignado | `409 Conflict` / `500` | `InvalidOperationException` en dominio |
| TC-05 | userId inválido (no GUID) | `404 Not Found` | Restricción de ruta `:guid` |
| TC-06 | roleId inválido (no GUID) | `404 Not Found` | Restricción de ruta `:guid` |
| TC-07 | Ambos IDs inválidos | `404 Not Found` | Restricción de ruta `:guid` |
| TC-08 | userId vacío (GUID zeros) | `404 Not Found` | `NotFoundException` en servicio |
| TC-09 | roleId vacío (GUID zeros) | `404 Not Found` | `NotFoundException` en servicio |

---

# UserController - `GET /api/users/{id}?showRoles={bool}`

## Descripción del endpoint

Obtiene un usuario por su ID. Opcionalmente incluye sus roles.

- **Método:** `GET`
- **URL:** `/api/users/{id:guid}?showRoles={bool}`
- **Controlador:** `UserController.GetById` (`src/TaskManager.Api/Controllers/UserController.cs:158`)
- **Servicio:** `IUserService.GetByIdAsync` / `IUserService.GetOneWithRolesAsync`

### Flujo de ejecución

1. Se recibe `id` como parámetro de ruta (GUID) y `showRoles` como query parameter (`bool`, por defecto `false`).
2. Si `showRoles == true`:
   - Se invoca `_userService.GetOneWithRolesAsync(id)`.
   - Si el usuario no existe → `404 Not Found`.
   - Si existe → `200 OK` con `UserWithRolesResponse` (incluye `Id`, `Username`, `FullName`, `Email`, `Avatar`, `AvatarBg`, `LastSession`, `CreatedAt`, `IsActive` y lista `Roles`).
3. Si `showRoles == false` (por defecto):
   - Se invoca `_userService.GetByIdAsync(id)`.
   - Si el usuario no existe → `404 Not Found`.
   - Si existe → `200 OK` con `UserResponse` (incluye `Id`, `Username`, `FullName`, `Email`, `CreatedAt`, `IsActive` — **sin** `Avatar`, `AvatarBg`, `LastSession`, `Roles`).

> **Nota:** El query parameter `showRoles` es opcional. Si se omite, su valor por defecto es `false`. El model binder de ASP.NET acepta `true`/`false` (string) y `1`/`0` (entero).

---

## Casos de prueba

### ✅ TC-10: Obtener usuario sin showRoles (por defecto false)

| Campo | Valor |
|---|---|
| **ID** | TC-10 |
| **Título** | Obtener usuario existente sin especificar `showRoles` |
| **Descripción** | Verifica que al obtener un usuario sin enviar el query parameter `showRoles` se retorne `200 OK` con `UserResponse` (sin roles). |
| **Precondiciones** | Existe un usuario con el `id` proporcionado. |
| **Datos de entrada** | `id` = GUID existente, sin query parameters |
| **Resultado esperado** | `200 OK` con `ApiResponse` donde `Data` es de tipo `UserResponse` (contiene `Id`, `Username`, `FullName`, `Email`, `CreatedAt`, `IsActive`). No incluye `Avatar`, `AvatarBg`, `LastSession` ni `Roles`. |
| **Mecanismo** | Se ejecuta la rama `showRoles == false` del controlador. |

---

### ✅ TC-11: Obtener usuario con showRoles=false explícito

| Campo | Valor |
|---|---|
| **ID** | TC-11 |
| **Título** | Obtener usuario existente con `showRoles=false` |
| **Descripción** | Verifica que al enviar `showRoles=false` explícitamente se retorne `200 OK` con `UserResponse` (sin roles), igual que TC-10. |
| **Precondiciones** | Existe un usuario con el `id` proporcionado. |
| **Datos de entrada** | `id` = GUID existente, `?showRoles=false` |
| **Resultado esperado** | `200 OK` con `ApiResponse` donde `Data` es de tipo `UserResponse` (misma estructura que TC-10). |
| **Mecanismo** | Se ejecuta la rama `showRoles == false` del controlador. |

---

### ✅ TC-12: Obtener usuario con showRoles=true (con roles asignados)

| Campo | Valor |
|---|---|
| **ID** | TC-12 |
| **Título** | Obtener usuario existente con `showRoles=true` — usuario tiene roles |
| **Descripción** | Verifica que al enviar `showRoles=true` y el usuario tiene roles asignados, se retorne `200 OK` con `UserWithRolesResponse` incluyendo la lista de roles. |
| **Precondiciones** | Existe un usuario con el `id` proporcionado. El usuario tiene uno o más roles asignados. |
| **Datos de entrada** | `id` = GUID existente, `?showRoles=true` |
| **Resultado esperado** | `200 OK` con `ApiResponse` donde `Data` es de tipo `UserWithRolesResponse`. El campo `Roles` contiene uno o más elementos con `Id`, `Name` y `Description`. La respuesta también incluye `Avatar`, `AvatarBg` y `LastSession`. |
| **Mecanismo** | Se ejecuta la rama `showRoles == true` del controlador. |

---

### ✅ TC-13: Obtener usuario con showRoles=true (sin roles asignados)

| Campo | Valor |
|---|---|
| **ID** | TC-13 |
| **Título** | Obtener usuario existente con `showRoles=true` — usuario sin roles |
| **Descripción** | Verifica que al enviar `showRoles=true` y el usuario no tiene roles asignados, se retorne `200 OK` con `UserWithRolesResponse` y lista de roles vacía. |
| **Precondiciones** | Existe un usuario con el `id` proporcionado. El usuario **no** tiene roles asignados. |
| **Datos de entrada** | `id` = GUID existente, `?showRoles=true` |
| **Resultado esperado** | `200 OK` con `ApiResponse` donde `Data` es de tipo `UserWithRolesResponse`. El campo `Roles` es una lista vacía `[]`. |
| **Mecanismo** | Se ejecuta la rama `showRoles == true` del controlador. |

---

### ✅ TC-14: Usuario no encontrado con showRoles=false

| Campo | Valor |
|---|---|
| **ID** | TC-14 |
| **Título** | Obtener usuario inexistente con `showRoles=false` |
| **Descripción** | Verifica que al intentar obtener un usuario que no existe con `showRoles=false` se retorne `404 Not Found`. |
| **Precondiciones** | No existe un usuario con el `id` proporcionado. |
| **Datos de entrada** | `id` = GUID válido pero inexistente, `?showRoles=false` |
| **Resultado esperado** | `404 Not Found` con `ApiResponse { Action = "get", HttpStatusCode = 404, Message = "User not found.", Data = null }` |
| **Mecanismo** | `GetByIdAsync` retorna `null`, el controlador retorna `NotFound`. |

---

### ✅ TC-15: Usuario no encontrado con showRoles=true

| Campo | Valor |
|---|---|
| **ID** | TC-15 |
| **Título** | Obtener usuario inexistente con `showRoles=true` |
| **Descripción** | Verifica que al intentar obtener un usuario que no existe con `showRoles=true` se retorne `404 Not Found`. |
| **Precondiciones** | No existe un usuario con el `id` proporcionado. |
| **Datos de entrada** | `id` = GUID válido pero inexistente, `?showRoles=true` |
| **Resultado esperado** | `404 Not Found` con `ApiResponse { Action = "get", HttpStatusCode = 404, Message = "User not found.", Data = null }` |
| **Mecanismo** | `GetOneWithRolesAsync` retorna `null`, el controlador retorna `NotFound`. |

---

### ❌ TC-16: id con formato inválido (no GUID)

| Campo | Valor |
|---|---|
| **ID** | TC-16 |
| **Título** | id con formato inválido |
| **Descripción** | Verifica que al proporcionar un `id` que no es un GUID válido se retorne un error. |
| **Precondiciones** | Ninguna. |
| **Datos de entrada** | `id` = `"not-a-guid"` |
| **Resultado esperado** | `404 Not Found` (la ruta no coincide debido a la restricción `:guid` en el template `{id:guid}`). |
| **Mecanismo** | La restricción de ruta `:guid` impide que ASP.NET Core matchee la ruta. |

---

### ❌ TC-17: id es un GUID vacío (all zeros)

| Campo | Valor |
|---|---|
| **ID** | TC-17 |
| **Título** | id con GUID vacío (00000000-0000-0000-0000-000000000000) |
| **Descripción** | Verifica que al proporcionar un GUID vacío como `id` se retorne `404 Not Found`. |
| **Precondiciones** | No existe un usuario con `Id = 00000000-0000-0000-0000-000000000000`. |
| **Datos de entrada** | `id` = `00000000-0000-0000-0000-000000000000` |
| **Resultado esperado** | `404 Not Found` con mensaje `"User not found."`. El GUID es sintácticamente válido, por lo que la ruta coincide, pero el servicio no encuentra al usuario. |
| **Mecanismo** | El servicio retorna `null`, el controlador retorna `NotFound`. |

---

### ❌ TC-18: showRoles con valor inválido (model binding)

| Campo | Valor |
|---|---|
| **ID** | TC-18 |
| **Título** | showRoles con valor no booleano |
| **Descripción** | Verifica que al enviar `showRoles` con un valor que no puede convertirse a `bool` se retorne `400 Bad Request`. |
| **Precondiciones** | Existe un usuario con el `id` proporcionado. |
| **Datos de entrada** | `id` = GUID existente, `?showRoles=invalid` |
| **Resultado esperado** | `400 Bad Request` con error de model binding indicando que el valor no es un `bool` válido. |
| **Mecanismo** | El model binder de ASP.NET no puede convertir `"invalid"` a `bool`, por lo que retorna un error de validación antes de ejecutar el controlador. |

---

### 🔶 TC-19: showRoles con valor "1" (entero aceptado como true)

| Campo | Valor |
|---|---|
| **ID** | TC-19 |
| **Título** | showRoles con valor `1` (interpretado como `true`) |
| **Descripción** | Verifica que el model binder de ASP.NET acepte `1` como `true` para el parámetro `showRoles`. |
| **Precondiciones** | Existe un usuario con el `id` proporcionado. |
| **Datos de entrada** | `id` = GUID existente, `?showRoles=1` |
| **Resultado esperado** | `200 OK` con `UserWithRolesResponse` (equivalente a TC-12 o TC-13 según si tiene roles). |
| **Mecanismo** | ASP.NET model binder interpreta `"1"` como `true` para tipo `bool`. |

---

### 🔶 TC-20: showRoles con valor "0" (entero aceptado como false)

| Campo | Valor |
|---|---|
| **ID** | TC-20 |
| **Título** | showRoles con valor `0` (interpretado como `false`) |
| **Descripción** | Verifica que el model binder de ASP.NET acepte `0` como `false` para el parámetro `showRoles`. |
| **Precondiciones** | Existe un usuario con el `id` proporcionado. |
| **Datos de entrada** | `id` = GUID existente, `?showRoles=0` |
| **Resultado esperado** | `200 OK` con `UserResponse` (equivalente a TC-10 y TC-11). |
| **Mecanismo** | ASP.NET model binder interpreta `"0"` como `false` para tipo `bool`. |

---

## Resumen de casos — `GET /api/users/{id}`

| ID | Escenario | HTTP Status esperado | Mecanismo |
|---|---|---|---|
| TC-10 | Obtener usuario sin `showRoles` (default false) | `200 OK` | Flujo feliz — `GetByIdAsync` |
| TC-11 | Obtener usuario con `showRoles=false` | `200 OK` | Flujo feliz — `GetByIdAsync` |
| TC-12 | Obtener usuario con `showRoles=true` (con roles) | `200 OK` | Flujo feliz — `GetOneWithRolesAsync` |
| TC-13 | Obtener usuario con `showRoles=true` (sin roles) | `200 OK` | Flujo feliz — `GetOneWithRolesAsync` con lista vacía |
| TC-14 | Usuario no encontrado, `showRoles=false` | `404 Not Found` | `GetByIdAsync` retorna `null` |
| TC-15 | Usuario no encontrado, `showRoles=true` | `404 Not Found` | `GetOneWithRolesAsync` retorna `null` |
| TC-16 | id inválido (no GUID) | `404 Not Found` | Restricción de ruta `:guid` |
| TC-17 | id vacío (GUID zeros) | `404 Not Found` | Servicio retorna `null` |
| TC-18 | `showRoles` con valor inválido | `400 Bad Request` | Model binding falla |
| TC-19 | `showRoles=1` (como true) | `200 OK` | Model binding interpreta `1` como `true` |
| TC-20 | `showRoles=0` (como false) | `200 OK` | Model binding interpreta `0` como `false` |

---

## Notas adicionales

### `POST /api/users/{userId}/roles/{roleId}`
- El controlador actualmente **no maneja explícitamente** las excepciones `NotFoundException` e `InvalidOperationException`. El código comentado (líneas 449-470) sugiere que se planeaba capturar `NotFoundException` para retornar `404`, pero está desactivado.
- Se recomienda decidir si las excepciones se manejan en el controlador (con `try/catch`) o globalmente (middleware), y de ser global, asegurar que `InvalidOperationException` retorne `409 Conflict` en lugar de `500`.
- Los tests deben considerar la existencia del middleware global de manejo de excepciones y cómo transforma cada tipo de excepción en la respuesta HTTP correspondiente.

### `GET /api/users/{id}`
- El endpoint soporta dos DTOs de respuesta distintos: `UserResponse` (sin roles, menos campos) y `UserWithRolesResponse` (con roles, más campos como `Avatar`, `AvatarBg`, `LastSession`). Los tests deben validar la estructura completa del `Data` según el valor de `showRoles`.
- El model binder de ASP.NET acepta `true`/`false` (case-insensitive) así como `1`/`0` para parámetros `bool`. Esto debe considerarse en pruebas de integración.
- La restricción de ruta `:guid` rechaza automáticamente valores no GUID con `404`, no con `400`. Esto es importante para las pruebas de validación de formato.
