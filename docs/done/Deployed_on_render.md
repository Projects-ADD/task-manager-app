Antes de desplegar la aplicación, hay que revisar los archivos de Docker ubicados en 

/docker

el archivo 
./docker/docker-compose.yml   en este escenario no servirá para el desplieque

El archivo más importante es:
./docker/Dockerfile.api  

y por lo tanto, la plataforma de render.com lo pedirá, así que es mejor proporcionar la ubicación tal y como está escrita.

En conclusión, los archivos Docker están listos para ser desplegados en producción.

-------------------------------------

Lo primero que se hace dentro de la plataforma de render.com es crear la base de datos:

Desde el Dashboard de render hay que seleccionar:
New --> Postgres

Esto dirige a un formulario para crear base de datos en PostgreSQL

Los datos proporcionados fueron:

Name:  taskmanager-db-render    # Este nombre se muestra sólo en render

Project: [Proyecto dentro de render al cual se quiere asociar]

Database: taskmanager   # CUIDADO! aquí render puede agregar un sufijo, en este caso lo renombró como taskmanager_1yin

Username : postgresqluser   # en render.com no permite el uso de 'postgres'

Password: <mpassword>   # Password generado por render.com

Internal Database URL: postgresql://postgresqluser:<mpassword>@dpg-d95jbt8js32c73fuic2g-a/taskmanager_1yin

External Database URL: postgresql://postgresqluser:<mpassword>@dpg-d95jbt8js32c73fuic2g-a.oregon-postgres.render.com/taskmanager_1yin

---
Hasta este punto, valdrá la pena extraer datos importantes, que pueden ser usados para conectarse a un cliente como 'dbeaver':

Host: dpg-d95jbt8js32c73fuic2g-a.oregon-postgres.render.com
Port: 5432
Username: postgresqluser
Password: <mpassword>

---

Si se quiere hacer la migración de EF, conviene modificar por 1 momento el archivo 

\src\TaskManager.Api\appsettings.Development.json

y colocar :
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=dpg-d95jbt8js32c73fuic2g-a.oregon-postgres.render.com;Port=5432;Database=taskmanager_1yin;Username=postgresqluser;Password=<mpassword>"
  },
  "Logging": {
    ...
  }
}

Cadena de conexión:
Host=dpg-d95jbt8js32c73fuic2g-a.oregon-postgres.render.com;Port=5432;Database=taskmanager_1yin;Username=postgresqluser;Password=<mpassword>

Cadena de conexión con SSL:
Host=dpg-d95jbt8js32c73fuic2g-a.oregon-postgres.render.com;Port=5432;Database=taskmanager_1yin;Username=postgresqluser;Password=<mpassword>;SSL Mode=Require;Trust Server 

guardar y ejecutar:

dotnet ef database update --project src/TaskManager.Infrastructure --startup-project src/TaskManager.Api

para realizar la migración. Con esto se crearán las tablas en la base de datos remota creada.

----

Luego, hay que crear el servicio web. Estos son los valores con los que se debe contar:

Branch: main
Dockerfile Path: ./docker/Dockerfile.api
Docker Context: repositorio raiz (.)
Health Check Path: /

Hay que definir las variables de entorno:

ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
ConnectionStrings__DefaultConnection=Host=dpg-d95jbt8js32c73fuic2g-a.oregon-postgres.render.com;Port=5432;Database=taskmanager_1yin;Username=postgresqluser;Password=<mpassword>

AL observar las variables, se asemejan mucho al archivo appsettings.json que maneja .Net

Render se encagará de desplegar automáticamente el servicio
----


El servicio de la API está en :
https://task-manager-app-3pn7.onrender.com


