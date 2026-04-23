# PortalAcademico

Sistema web desarrollado con ASP.NET Core MVC.

## Tecnologías
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core (SQLite)
- Identity
- Redis (cache distribuido)
- Docker
- Render

## Funcionalidades
- Registro e inicio de sesión
- Catálogo de cursos
- Inscripción de estudiantes
- Panel de coordinador (CRUD + gestión de matrículas)

## Despliegue en Render

### URL de la aplicación
https://portalacademico-y3bi.onrender.com

### Variables de entorno

```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:8080

ConnectionStrings__DefaultConnection=Data Source=app.db
Redis__ConnectionString=red-d7l9v6t7vvec73e4c7ig:6379
```

### Explicación de variables

- **ASPNETCORE_ENVIRONMENT**: Define el entorno de ejecución (Producción).
- **ASPNETCORE_URLS**: Configura el puerto donde la app escucha en Render.
- **ConnectionStrings__DefaultConnection**: Cadena de conexión a SQLite.
- **Redis__ConnectionString**: Conexión a Redis (KeyValue de Render).

### Redis
Se utilizó el servicio **KeyValue de Render** como implementación de Redis.

### Docker
El despliegue se realizó utilizando un **Dockerfile en la raíz del proyecto**.

## Pruebas realizadas

- Login funcionando
- Catálogo de cursos operativo
- Inscripción correcta
- Panel de coordinador funcional
- Cache Redis funcionando

## Credenciales de prueba

### Coordinador
- Correo: coordinador@portal.com
- Contraseña: Coordinador123!

## Repositorio
https://github.com/yordan347-cpu/PortalAcademico
