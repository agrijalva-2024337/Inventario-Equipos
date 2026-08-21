using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InventarioEquipos.Persistence.Data;

/// <summary>
/// Fábrica de tiempo de diseño para las herramientas de EF Core
/// (dotnet ef migrations add / database update). El DbContext vive en un
/// proyecto de librería (Persistence) y el proyecto de arranque (Api) usa
/// hosting mínimo con top-level statements; la herramienta no siempre logra
/// levantar ese host solo para obtener el DbContext, así que le damos esta
/// forma explícita de construirlo.
///
/// La cadena de conexión de aquí es SOLO para generar/aplicar migraciones
/// desde la terminal — no se usa en tiempo de ejecución real. La app real
/// sigue registrando el DbContext en Program.cs con la cadena de conexión
/// de user-secrets / appsettings, tal como ya está.
/// </summary>
public class InventarioEquiposDbContextFactory : IDesignTimeDbContextFactory<InventarioEquiposDbContext>
{
    public InventarioEquiposDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<InventarioEquiposDbContext>();

        // Ajusta esta cadena a tu instancia local de SQL Server si la
        // necesitas distinta (server/instancia, usuario/password, etc.).
        // Solo se usa para que dotnet ef pueda construir el modelo y
        // generar el SQL de la migración; no requiere que la base exista.
        optionsBuilder.UseSqlServer(
            "Server=localhost;Database=InventarioEquipos;Trusted_Connection=True;TrustServerCertificate=True;");

        return new InventarioEquiposDbContext(optionsBuilder.Options);
    }
}
