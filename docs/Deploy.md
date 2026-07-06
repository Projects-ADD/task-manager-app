Para hacer el deploy, se hará uso de los servicios:

API-REST:  render.com
Base de datos:  render.com

Como alternativa:

API-REST:  render.com
Base de datos:  neon.tech


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