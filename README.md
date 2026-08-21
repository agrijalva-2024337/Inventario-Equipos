# Inventario-Equipos
Web diseñada para llevar el registro y el estado de los equipos utilizados en la empresa SLCtrade

## Configuración local (cadena de conexión)

La cadena de SQL Server **no se commitea**. Va en [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) del proyecto API.

Desde la raíz del repo:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=InventarioEquipos;Trusted_Connection=True;TrustServerCertificate=True;" --project src/InventarioEquipos.Api
```

Ajustá el valor de `Server`, `Database` y autenticación a tu instancia local. `appsettings.json` deja `DefaultConnection` vacío a propósito; en Development, user-secrets pisa esa clave.

Para listar lo configurado (sin subirlo a git):

```bash
dotnet user-secrets list --project src/InventarioEquipos.Api
```
