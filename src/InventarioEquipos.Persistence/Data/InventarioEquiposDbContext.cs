using InventarioEquipos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventarioEquipos.Persistence.Data;

/// <summary>
/// DbContext del núcleo multiempresa y seguridad. Conserva PascalCase en
/// las clases/propiedades de C# y convierte a snake_case solo a nivel de
/// nombres de tabla/columna/FK/índice en la base de datos, con el mismo
/// conversor por reflexión que ya usan en AuthService.Persistence.Data.ApplicationDbContext
/// (en vez del paquete EFCore.NamingConventions).
/// </summary>
public class InventarioEquiposDbContext(DbContextOptions<InventarioEquiposDbContext> options)
    : DbContext(options)
{
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Pais> Paises => Set<Pais>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<UsuarioEmpresa> UsuarioEmpresas => Set<UsuarioEmpresa>();
    public DbSet<HistorialCambio> HistorialCambios => Set<HistorialCambio>();
    public DbSet<Sede> Sedes => Set<Sede>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
            entity.Property(e => e.NitCodigo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.Telefono).HasMaxLength(30);
            entity.Property(e => e.Estado).HasConversion<string>().HasMaxLength(20);

            entity.HasIndex(e => e.NitCodigo).IsUnique();
        });

        modelBuilder.Entity<Pais>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CodigoIso2).IsRequired().HasMaxLength(2);
            entity.Property(e => e.CodigoIso3).IsRequired().HasMaxLength(3);
            entity.Property(e => e.CodigoTelefonico).IsRequired().HasMaxLength(10);
            entity.Property(e => e.MonedaLocal).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Estado).HasConversion<string>().HasMaxLength(20);

            entity.HasIndex(e => e.CodigoIso2).IsUnique();
            entity.HasIndex(e => e.CodigoIso3).IsUnique();
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.NombreCompleto).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Correo).IsRequired().HasMaxLength(150);
            entity.Property(e => e.UsuarioLogin).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Estado).HasConversion<string>().HasMaxLength(20);

            entity.HasIndex(e => e.Correo).IsUnique();
            entity.HasIndex(e => e.UsuarioLogin).IsUnique();
        });

        modelBuilder.Entity<UsuarioEmpresa>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Rol).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(ue => ue.Usuario)
                .WithMany()
                .HasForeignKey(ue => ue.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ue => ue.Empresa)
                .WithMany()
                .HasForeignKey(ue => ue.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Un mismo usuario no puede tener dos filas para la misma empresa.
            entity.HasIndex(ue => new { ue.UsuarioId, ue.EmpresaId }).IsUnique();
        });

        modelBuilder.Entity<HistorialCambio>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.TipoOperacion).HasConversion<string>().HasMaxLength(30);
            entity.Property(e => e.EntidadAfectada).IsRequired().HasMaxLength(100);
            entity.Property(e => e.InformacionAnterior).HasColumnType("nvarchar(max)");
            entity.Property(e => e.InformacionNueva).HasColumnType("nvarchar(max)");

            entity.HasOne(h => h.Usuario)
                .WithMany()
                .HasForeignKey(h => h.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(h => h.Empresa)
                .WithMany()
                .HasForeignKey(h => h.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Sede>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.Ciudad).HasMaxLength(100);
            entity.Property(e => e.Estado).HasConversion<string>().HasMaxLength(20);

            entity.HasOne(s => s.Empresa)
                .WithMany()
                .HasForeignKey(s => s.IdEmpresa)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.Pais)
                .WithMany()
                .HasForeignKey(s => s.IdPais)
                .OnDelete(DeleteBehavior.Restrict);
        });

        AplicarConvencionSnakeCase(modelBuilder);
    }

    /// <summary>
    /// Convierte tablas, columnas, claves foráneas e índices a snake_case,
    /// sin tocar los nombres de las clases/propiedades en C# (que se quedan
    /// en PascalCase). Mismo patrón que
    /// AuthService.Persistence.Data.ApplicationDbContext.ToSnakeCase, para
    /// que ambos microservicios generen bases de datos con la misma
    /// convención de nombres.
    /// </summary>
    private static void AplicarConvencionSnakeCase(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
                entity.SetTableName(ToSnakeCase(tableName));

            foreach (var property in entity.GetProperties())
            {
                var columnName = property.GetColumnName();
                if (!string.IsNullOrEmpty(columnName))
                    property.SetColumnName(ToSnakeCase(columnName));
            }

            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName();
                if (!string.IsNullOrEmpty(keyName))
                    key.SetName(ToSnakeCase(keyName));
            }

            foreach (var foreignKey in entity.GetForeignKeys())
            {
                var fkName = foreignKey.GetConstraintName();
                if (!string.IsNullOrEmpty(fkName))
                    foreignKey.SetConstraintName(ToSnakeCase(fkName));
            }

            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if (!string.IsNullOrEmpty(indexName))
                    index.SetDatabaseName(ToSnakeCase(indexName));
            }
        }
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return string.Concat(
            input.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + c : c.ToString())
        ).ToLower();
    }
}
