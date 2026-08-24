using InventarioEquipos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventarioEquipos.Persistence.Data;

/// <summary>
/// DbContext de todo el sistema DERCAS. A diferencia de versiones
/// anteriores, este NO usa un conversor genérico de PascalCase→snake_case:
/// el equipo adoptó como fuente de verdad el script escrito a mano
/// <c>InventarioMultiempresa.sql</c> (raíz del repo), cuyos nombres de
/// tabla no son un snake_case plural regular (son singulares, con
/// PascalCase y guion bajo: "Categoria_Activo", "Usuario_Empresa", etc.),
/// así que cada tabla y cada columna se mapean explícitamente con
/// <c>ToTable(...)</c> / <c>HasColumnName(...)</c> para que coincidan
/// carácter por carácter con ese script.
///
/// Importante: esto asume que <c>InventarioMultiempresa.sql</c> ya
/// corrigió el typo original "codigot_telefonico" → "codigo_telefonico"
/// (ver notas en el PR/instructivo). Si no se corrige ahí, hay que
/// cambiar también el nombre de columna aquí para que coincida.
/// </summary>
public class InventarioEquiposDbContext(DbContextOptions<InventarioEquiposDbContext> options)
    : DbContext(options)
{
    public DbSet<Pais> Paises => Set<Pais>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<UsuarioEmpresa> UsuarioEmpresas => Set<UsuarioEmpresa>();
    public DbSet<Sede> Sedes => Set<Sede>();
    public DbSet<Ubicacion> Ubicaciones => Set<Ubicacion>();
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<Responsable> Responsables => Set<Responsable>();
    public DbSet<CategoriaActivo> CategoriasActivo => Set<CategoriaActivo>();
    public DbSet<EstadoActivo> EstadosActivo => Set<EstadoActivo>();
    public DbSet<TipoMantenimiento> TiposMantenimiento => Set<TipoMantenimiento>();
    public DbSet<MotivoBaja> MotivosBaja => Set<MotivoBaja>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Activo> Activos => Set<Activo>();
    public DbSet<Asignacion> Asignaciones => Set<Asignacion>();
    public DbSet<Traslado> Traslados => Set<Traslado>();
    public DbSet<Mantenimiento> Mantenimientos => Set<Mantenimiento>();
    public DbSet<Baja> Bajas => Set<Baja>();
    public DbSet<InventarioFisico> InventariosFisicos => Set<InventarioFisico>();
    public DbSet<DetalleInventario> DetallesInventario => Set<DetalleInventario>();
    public DbSet<HistorialCambio> HistorialCambios => Set<HistorialCambio>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Pais>(entity =>
        {
            entity.ToTable("Pais");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_pais").ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).HasColumnName("nombre").IsRequired().HasMaxLength(100);
            entity.Property(e => e.CodigoIso2).HasColumnName("codigo_iso2").IsRequired().HasMaxLength(2);
            entity.Property(e => e.CodigoIso3).HasColumnName("codigo_iso3").IsRequired().HasMaxLength(3);
            entity.Property(e => e.CodigoTelefonico).HasColumnName("codigo_telefonico").HasMaxLength(5);
            entity.Property(e => e.MonedaLocal).HasColumnName("moneda_local").HasMaxLength(10);
            entity.Property(e => e.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);

            // No están en el .sql como UNIQUE, pero tiene sentido de negocio
            // que no se repitan dos países con el mismo código ISO.
            entity.HasIndex(e => e.CodigoIso2).IsUnique();
            entity.HasIndex(e => e.CodigoIso3).IsUnique();
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_usuario").ValueGeneratedOnAdd();
            entity.Property(e => e.NombreCompleto).HasColumnName("nombre_completo").IsRequired().HasMaxLength(150);
            entity.Property(e => e.Correo).HasColumnName("correo").IsRequired().HasMaxLength(150);
            entity.Property(e => e.UsuarioLogin).HasColumnName("usuario_login").IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").IsRequired().HasMaxLength(255);
            entity.Property(e => e.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");

            entity.HasIndex(e => e.Correo).IsUnique().HasDatabaseName("UQ_Usuario_Correo");
            entity.HasIndex(e => e.UsuarioLogin).IsUnique().HasDatabaseName("UQ_Usuario_Login");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.ToTable("Empresa");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_empresa").ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).HasColumnName("nombre").IsRequired().HasMaxLength(150);
            entity.Property(e => e.NitCodigo).HasColumnName("nit_codigo").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Direccion).HasColumnName("direccion").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Telefono).HasColumnName("telefono").HasMaxLength(30);
            entity.Property(e => e.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");

            entity.HasIndex(e => e.NitCodigo).IsUnique().HasDatabaseName("UQ_Empresa_Nit");
        });

        modelBuilder.Entity<UsuarioEmpresa>(entity =>
        {
            entity.ToTable("Usuario_Empresa");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_usuario_empresa").ValueGeneratedOnAdd();
            entity.Property(e => e.Rol).HasColumnName("rol").HasConversion<string>().IsRequired().HasMaxLength(50);
            entity.Property(e => e.EmpresaPredeterminada).HasColumnName("empresa_predeterminada");
            entity.Property(e => e.FechaAsignacion).HasColumnName("fecha_asignacion");

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.HasOne(ue => ue.Usuario)
                .WithMany()
                .HasForeignKey(ue => ue.IdUsuario)
                .HasConstraintName("FK_UsuarioEmpresa_Usuario")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.HasOne(ue => ue.Empresa)
                .WithMany()
                .HasForeignKey(ue => ue.IdEmpresa)
                .HasConstraintName("FK_UsuarioEmpresa_Empresa")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(ue => new { ue.IdUsuario, ue.IdEmpresa })
                .IsUnique()
                .HasDatabaseName("UQ_UsuarioEmpresa");
        });

        modelBuilder.Entity<Sede>(entity =>
        {
            entity.ToTable("Sede");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_sede").ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).HasColumnName("nombre").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Direccion).HasColumnName("direccion").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Ciudad).HasColumnName("ciudad").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);

            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.HasOne(s => s.Empresa)
                .WithMany()
                .HasForeignKey(s => s.IdEmpresa)
                .HasConstraintName("FK_Sede_Empresa")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdPais).HasColumnName("id_pais");
            entity.HasOne(s => s.Pais)
                .WithMany()
                .HasForeignKey(s => s.IdPais)
                .HasConstraintName("FK_Sede_Pais")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Ubicacion>(entity =>
        {
            entity.ToTable("Ubicacion");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_ubicacion").ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).HasColumnName("nombre").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(200);
            entity.Property(e => e.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);

            entity.Property(e => e.IdSede).HasColumnName("id_sede");
            entity.HasOne(u => u.Sede)
                .WithMany()
                .HasForeignKey(u => u.IdSede)
                .HasConstraintName("FK_Ubicacion_Sede")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.ToTable("Area");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_area").ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).HasColumnName("nombre").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(200);
            entity.Property(e => e.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);

            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.HasOne(a => a.Empresa)
                .WithMany()
                .HasForeignKey(a => a.IdEmpresa)
                .HasConstraintName("FK_Area_Empresa")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Responsable>(entity =>
        {
            entity.ToTable("Responsable");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_responsable").ValueGeneratedOnAdd();
            entity.Property(e => e.NombreCompleto).HasColumnName("nombre_completo").IsRequired().HasMaxLength(150);
            entity.Property(e => e.Cargo).HasColumnName("cargo").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Correo).HasColumnName("correo").IsRequired().HasMaxLength(150);
            entity.Property(e => e.Telefono).HasColumnName("telefono").IsRequired().HasMaxLength(30);
            entity.Property(e => e.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);

            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.HasOne(r => r.Empresa)
                .WithMany()
                .HasForeignKey(r => r.IdEmpresa)
                .HasConstraintName("FK_Responsable_Empresa")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdArea).HasColumnName("id_area");
            entity.HasOne(r => r.Area)
                .WithMany()
                .HasForeignKey(r => r.IdArea)
                .HasConstraintName("FK_Responsable_Area")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.IdUsuario)
                .HasConstraintName("FK_Responsable_Usuario")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CategoriaActivo>(entity =>
        {
            entity.ToTable("Categoria_Activo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_categoria").ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).HasColumnName("nombre").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(200);
            entity.Property(e => e.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);

            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.HasOne(c => c.Empresa)
                .WithMany()
                .HasForeignKey(c => c.IdEmpresa)
                .HasConstraintName("FK_Categoria_Empresa")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EstadoActivo>(entity =>
        {
            entity.ToTable("Estado_Activo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_estado").ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).HasColumnName("nombre").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(150);

            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.HasOne(e => e.Empresa)
                .WithMany()
                .HasForeignKey(e => e.IdEmpresa)
                .HasConstraintName("FK_EstadoActivo_Empresa")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TipoMantenimiento>(entity =>
        {
            entity.ToTable("Tipo_Mantenimiento");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_tipo_mantenimiento").ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).HasColumnName("nombre").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(150);

            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.HasOne(t => t.Empresa)
                .WithMany()
                .HasForeignKey(t => t.IdEmpresa)
                .HasConstraintName("FK_TipoMantenimiento_Empresa")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MotivoBaja>(entity =>
        {
            entity.ToTable("Motivo_Baja");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_motivo_baja").ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).HasColumnName("nombre").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(150);

            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.HasOne(m => m.Empresa)
                .WithMany()
                .HasForeignKey(m => m.IdEmpresa)
                .HasConstraintName("FK_MotivoBaja_Empresa")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.ToTable("Proveedor");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_proveedor").ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).HasColumnName("nombre").IsRequired().HasMaxLength(150);
            entity.Property(e => e.Nit).HasColumnName("nit").HasMaxLength(50);
            entity.Property(e => e.Contacto).HasColumnName("contacto").HasMaxLength(100);
            entity.Property(e => e.Telefono).HasColumnName("telefono").IsRequired().HasMaxLength(30);
            entity.Property(e => e.Correo).HasColumnName("correo").HasMaxLength(150);
            entity.Property(e => e.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);

            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.HasOne(p => p.Empresa)
                .WithMany()
                .HasForeignKey(p => p.IdEmpresa)
                .HasConstraintName("FK_Proveedor_Empresa")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Activo>(entity =>
        {
            entity.ToTable("Activo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_activo").ValueGeneratedOnAdd();
            entity.Property(e => e.CodigoInterno).HasColumnName("codigo_interno").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Nombre).HasColumnName("nombre").IsRequired().HasMaxLength(150);
            entity.Property(e => e.Descripcion).HasColumnName("descripcion").IsRequired().HasMaxLength(300);
            entity.Property(e => e.Marca).HasColumnName("marca").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Modelo).HasColumnName("modelo").IsRequired().HasMaxLength(100);
            entity.Property(e => e.NumeroSerie).HasColumnName("numero_serie").IsRequired().HasMaxLength(100);
            entity.Property(e => e.FechaCompra).HasColumnName("fecha_compra");
            entity.Property(e => e.CostoAdquisicion).HasColumnName("costo_adquisicion").HasColumnType("decimal(12,2)");
            entity.Property(e => e.Moneda).HasColumnName("moneda").HasMaxLength(10);
            entity.Property(e => e.NumeroFactura).HasColumnName("numero_factura").HasMaxLength(50);
            entity.Property(e => e.FechaVencimientoGarantia).HasColumnName("fecha_vencimiento_garantia");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones").HasMaxLength(500);

            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.HasOne(a => a.Empresa)
                .WithMany()
                .HasForeignKey(a => a.IdEmpresa)
                .HasConstraintName("FK_Activo_Empresa")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.HasOne(a => a.CategoriaActivo)
                .WithMany()
                .HasForeignKey(a => a.IdCategoria)
                .HasConstraintName("FK_Activo_Categoria")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.HasOne(a => a.Proveedor)
                .WithMany()
                .HasForeignKey(a => a.IdProveedor)
                .HasConstraintName("FK_Activo_Proveedor")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdSede).HasColumnName("id_sede");
            entity.HasOne(a => a.Sede)
                .WithMany()
                .HasForeignKey(a => a.IdSede)
                .HasConstraintName("FK_Activo_Sede")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdUbicacion).HasColumnName("id_ubicacion");
            entity.HasOne(a => a.Ubicacion)
                .WithMany()
                .HasForeignKey(a => a.IdUbicacion)
                .HasConstraintName("FK_Activo_Ubicacion")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdArea).HasColumnName("id_area");
            entity.HasOne(a => a.Area)
                .WithMany()
                .HasForeignKey(a => a.IdArea)
                .HasConstraintName("FK_Activo_Area")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdResponsable).HasColumnName("id_responsable");
            entity.HasOne(a => a.Responsable)
                .WithMany()
                .HasForeignKey(a => a.IdResponsable)
                .HasConstraintName("FK_Activo_Responsable")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdEstado).HasColumnName("id_estado");
            entity.HasOne(a => a.EstadoActivo)
                .WithMany()
                .HasForeignKey(a => a.IdEstado)
                .HasConstraintName("FK_Activo_Estado")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(a => new { a.IdEmpresa, a.CodigoInterno })
                .IsUnique()
                .HasDatabaseName("UQ_Activo_CodigoInterno");
        });

        modelBuilder.Entity<Asignacion>(entity =>
        {
            entity.ToTable("Asignacion");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_asignacion").ValueGeneratedOnAdd();
            entity.Property(e => e.FechaAsignacion).HasColumnName("fecha_asignacion");
            entity.Property(e => e.EntregadoPor).HasColumnName("entregado_por").IsRequired().HasMaxLength(150);
            entity.Property(e => e.RecibidoPor).HasColumnName("recibido_por").IsRequired().HasMaxLength(150);
            entity.Property(e => e.FechaDevolucion).HasColumnName("fecha_devolucion");
            entity.Property(e => e.Activa).HasColumnName("activa");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones").HasMaxLength(300);

            entity.Property(e => e.IdActivo).HasColumnName("id_activo");
            entity.HasOne(a => a.Activo)
                .WithMany()
                .HasForeignKey(a => a.IdActivo)
                .HasConstraintName("FK_Asignacion_Activo")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdResponsable).HasColumnName("id_responsable");
            entity.HasOne(a => a.Responsable)
                .WithMany()
                .HasForeignKey(a => a.IdResponsable)
                .HasConstraintName("FK_Asignacion_Responsable")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdUbicacionUso).HasColumnName("id_ubicacion_uso");
            entity.HasOne(a => a.UbicacionUso)
                .WithMany()
                .HasForeignKey(a => a.IdUbicacionUso)
                .HasConstraintName("FK_Asignacion_Ubicacion")
                .OnDelete(DeleteBehavior.Restrict);

            // No está en el .sql, pero es la regla del DERCAS ("un activo
            // solo puede tener una asignación activa"): índice único
            // filtrado, solo aplica mientras activa = 1.
            entity.HasIndex(a => a.IdActivo)
                .IsUnique()
                .HasDatabaseName("UQ_Asignacion_Activo_Activa")
                .HasFilter("[activa] = 1");
        });

        modelBuilder.Entity<Traslado>(entity =>
        {
            entity.ToTable("Traslado");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_traslado").ValueGeneratedOnAdd();
            entity.Property(e => e.FechaTraslado).HasColumnName("fecha_traslado");
            entity.Property(e => e.Motivo).HasColumnName("motivo").HasMaxLength(200);
            entity.Property(e => e.ResponsableTraslado).HasColumnName("responsable_traslado").HasMaxLength(150);

            entity.Property(e => e.IdActivo).HasColumnName("id_activo");
            entity.HasOne(t => t.Activo)
                .WithMany()
                .HasForeignKey(t => t.IdActivo)
                .HasConstraintName("FK_Traslado_Activo")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdUbicacionOrigen).HasColumnName("id_ubicacion_origen");
            entity.HasOne(t => t.UbicacionOrigen)
                .WithMany()
                .HasForeignKey(t => t.IdUbicacionOrigen)
                .HasConstraintName("FK_Traslado_Origen")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdUbicacionDestino).HasColumnName("id_ubicacion_destino");
            entity.HasOne(t => t.UbicacionDestino)
                .WithMany()
                .HasForeignKey(t => t.IdUbicacionDestino)
                .HasConstraintName("FK_Traslado_Destino")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Mantenimiento>(entity =>
        {
            entity.ToTable("Mantenimiento");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_mantenimiento").ValueGeneratedOnAdd();
            entity.Property(e => e.FechaProgramada).HasColumnName("fecha_programada");
            entity.Property(e => e.FechaRealizado).HasColumnName("fecha_realizado");
            entity.Property(e => e.Responsable).HasColumnName("responsable").HasMaxLength(150);
            entity.Property(e => e.DescripcionProblema).HasColumnName("descripcion_problema").HasMaxLength(300);
            entity.Property(e => e.TrabajoRealizado).HasColumnName("trabajo_realizado").HasMaxLength(150);
            entity.Property(e => e.Costo).HasColumnName("costo").HasColumnType("decimal(12,2)");
            entity.Property(e => e.NumeroFactura).HasColumnName("numero_factura").HasMaxLength(50);
            entity.Property(e => e.EstadoMantenimiento).HasColumnName("estado_mantenimiento").HasConversion<string>().HasMaxLength(30);

            entity.Property(e => e.IdActivo).HasColumnName("id_activo");
            entity.HasOne(m => m.Activo)
                .WithMany()
                .HasForeignKey(m => m.IdActivo)
                .HasConstraintName("FK_Mantenimiento_Activo")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdTipoMantenimiento).HasColumnName("id_tipo_mantenimiento");
            entity.HasOne(m => m.TipoMantenimiento)
                .WithMany()
                .HasForeignKey(m => m.IdTipoMantenimiento)
                .HasConstraintName("FK_Mantenimiento_Tipo")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.HasOne(m => m.Proveedor)
                .WithMany()
                .HasForeignKey(m => m.IdProveedor)
                .HasConstraintName("FK_Mantenimiento_Proveedor")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Baja>(entity =>
        {
            entity.ToTable("Baja");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_baja").ValueGeneratedOnAdd();
            entity.Property(e => e.FechaBaja).HasColumnName("fecha_baja");
            entity.Property(e => e.DocumentoReferencia).HasColumnName("documento_referencia").HasMaxLength(100);
            entity.Property(e => e.AutorizadoPor).HasColumnName("autorizado_por").HasMaxLength(150);
            entity.Property(e => e.Observaciones).HasColumnName("observaciones").HasMaxLength(300);

            entity.Property(e => e.IdActivo).HasColumnName("id_activo");
            entity.HasOne(b => b.Activo)
                .WithMany()
                .HasForeignKey(b => b.IdActivo)
                .HasConstraintName("FK_Baja_Activo")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdMotivoBaja).HasColumnName("id_motivo_baja");
            entity.HasOne(b => b.MotivoBaja)
                .WithMany()
                .HasForeignKey(b => b.IdMotivoBaja)
                .HasConstraintName("FK_Baja_MotivoBaja")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InventarioFisico>(entity =>
        {
            entity.ToTable("Inventario_Fisico");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_inventario").ValueGeneratedOnAdd();
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.FechaCierre).HasColumnName("fecha_cierre");
            entity.Property(e => e.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Observaciones).HasColumnName("observaciones").HasMaxLength(300);

            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.HasOne(i => i.Empresa)
                .WithMany()
                .HasForeignKey(i => i.IdEmpresa)
                .HasConstraintName("FK_InventarioFisico_Empresa")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdSede).HasColumnName("id_sede");
            entity.HasOne(i => i.Sede)
                .WithMany()
                .HasForeignKey(i => i.IdSede)
                .HasConstraintName("FK_InventarioFisico_Sede")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdUbicacion).HasColumnName("id_ubicacion");
            entity.HasOne(i => i.Ubicacion)
                .WithMany()
                .HasForeignKey(i => i.IdUbicacion)
                .HasConstraintName("FK_InventarioFisico_Ubicacion")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdUsuarioResponsable).HasColumnName("id_usuario_responsable");
            entity.HasOne(i => i.UsuarioResponsable)
                .WithMany()
                .HasForeignKey(i => i.IdUsuarioResponsable)
                .HasConstraintName("FK_InventarioFisico_Usuario")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DetalleInventario>(entity =>
        {
            entity.ToTable("Detalle_Inventario");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_detalle").ValueGeneratedOnAdd();
            entity.Property(e => e.Encontrado).HasColumnName("encontrado");
            entity.Property(e => e.EstadoFisicoObservado).HasColumnName("estado_fisico_observado").HasMaxLength(100);
            entity.Property(e => e.Observaciones).HasColumnName("observaciones").HasMaxLength(300);
            entity.Property(e => e.FechaVerificacion).HasColumnName("fecha_verificacion");

            entity.Property(e => e.IdInventario).HasColumnName("id_inventario");
            entity.HasOne(d => d.InventarioFisico)
                .WithMany()
                .HasForeignKey(d => d.IdInventario)
                .HasConstraintName("FK_DetalleInventario_Inventario")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdActivo).HasColumnName("id_activo");
            entity.HasOne(d => d.Activo)
                .WithMany()
                .HasForeignKey(d => d.IdActivo)
                .HasConstraintName("FK_DetalleInventario_Activo")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdUbicacionEncontrada).HasColumnName("id_ubicacion_encontrada");
            entity.HasOne(d => d.UbicacionEncontrada)
                .WithMany()
                .HasForeignKey(d => d.IdUbicacionEncontrada)
                .HasConstraintName("FK_DetalleInventario_Ubicacion")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<HistorialCambio>(entity =>
        {
            entity.ToTable("Historial_Cambios");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_historial").ValueGeneratedOnAdd();
            entity.Property(e => e.FechaHora).HasColumnName("fecha_hora");
            entity.Property(e => e.TipoOperacion).HasColumnName("tipo_operacion").HasConversion<string>().IsRequired().HasMaxLength(30);
            entity.Property(e => e.EntidadAfectada).HasColumnName("entidad_afectada").IsRequired().HasMaxLength(100);
            entity.Property(e => e.IdRegistroAfectado).HasColumnName("id_registro_afectado");
            entity.Property(e => e.InformacionAnterior).HasColumnName("informacion_anterior").HasColumnType("nvarchar(max)");
            entity.Property(e => e.InformacionNueva).HasColumnName("informacion_nueva").HasColumnType("nvarchar(max)");

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.HasOne(h => h.Usuario)
                .WithMany()
                .HasForeignKey(h => h.IdUsuario)
                .HasConstraintName("FK_HistorialCambios_Usuario")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.HasOne(h => h.Empresa)
                .WithMany()
                .HasForeignKey(h => h.IdEmpresa)
                .HasConstraintName("FK_HistorialCambios_Empresa")
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
