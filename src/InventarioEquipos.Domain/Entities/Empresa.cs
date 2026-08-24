using InventarioEquipos.Domain.Common;
using InventarioEquipos.Domain.Enums;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Empresa: entidad raíz del núcleo multiempresa. Casi todas las demás
/// entidades del sistema tendrán FK hacia Empresa (id_empresa), por eso
/// va primero. Columnas según el diagrama: nombre, nit_codigo, direccion
/// (VARCHAR 50 NOT NULL), telefono (opcional), estado, fecha_creacion.
/// </summary>
public class Empresa : EntityBase
{
    public string Nombre { get; private set; } = default!;
    public string NitCodigo { get; private set; } = default!;
    public string Direccion { get; private set; } = default!;
    public string? Telefono { get; private set; }
    public EstadoRegistro Estado { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    protected Empresa() { } // Para ORMs / serialización.

    private Empresa(
        string nombre,
        string nitCodigo,
        string direccion,
        string? telefono)
    {
        Nombre = nombre;
        NitCodigo = nitCodigo;
        Direccion = direccion;
        Telefono = telefono;
        Estado = EstadoRegistro.Activo;
        FechaCreacion = DateTime.UtcNow;
    }

    public static Empresa Crear(
        string nombre,
        string nitCodigo,
        string direccion,
        string? telefono = null)
    {
        ValidarNombre(nombre);
        ValidarNitCodigo(nitCodigo);
        ValidarDireccion(direccion);
        ValidarTelefono(telefono);

        return new Empresa(nombre.Trim(), nitCodigo.Trim(), direccion.Trim(), NormalizarTelefono(telefono));
    }

    public void ActualizarDatos(
        string nombre,
        string nitCodigo,
        string direccion,
        string? telefono)
    {
        ValidarNombre(nombre);
        ValidarNitCodigo(nitCodigo);
        ValidarDireccion(direccion);
        ValidarTelefono(telefono);

        Nombre = nombre.Trim();
        NitCodigo = nitCodigo.Trim();
        Direccion = direccion.Trim();
        Telefono = NormalizarTelefono(telefono);
    }

    public void Activar() => Estado = EstadoRegistro.Activo;

    public void Desactivar() => Estado = EstadoRegistro.Inactivo;

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la empresa es obligatorio.", nameof(nombre));
        if (nombre.Length > 150)
            throw new ArgumentException("El nombre de la empresa no puede exceder 150 caracteres.", nameof(nombre));
    }

    private static void ValidarNitCodigo(string nitCodigo)
    {
        if (string.IsNullOrWhiteSpace(nitCodigo))
            throw new ArgumentException("El NIT/código de la empresa es obligatorio.", nameof(nitCodigo));
        if (nitCodigo.Length > 50)
            throw new ArgumentException("El NIT/código no puede exceder 50 caracteres.", nameof(nitCodigo));
    }

    private static void ValidarDireccion(string direccion)
    {
        if (string.IsNullOrWhiteSpace(direccion))
            throw new ArgumentException("La dirección de la empresa es obligatoria.", nameof(direccion));
        if (direccion.Trim().Length > 50)
            throw new ArgumentException("La dirección de la empresa no puede exceder 50 caracteres.", nameof(direccion));
    }

    private static void ValidarTelefono(string? telefono)
    {
        if (telefono is not null && telefono.Trim().Length > 30)
            throw new ArgumentException("El teléfono no puede exceder 30 caracteres.", nameof(telefono));
    }

    private static string? NormalizarTelefono(string? telefono)
        => string.IsNullOrWhiteSpace(telefono) ? null : telefono.Trim();
}
