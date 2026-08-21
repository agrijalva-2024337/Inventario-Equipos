using InventarioEquipos.Domain.Common;
using InventarioEquipos.Domain.Enums;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Ubicación física dentro de una sede. Depende de Sede. Columnas según
/// el diagrama: id_sede, nombre, descripcion, estado.
/// </summary>
public class Ubicacion : EntityBase
{
    public int SedeId { get; private set; }
    public Sede? Sede { get; private set; }

    public string Nombre { get; private set; } = default!;
    public string? Descripcion { get; private set; }
    public EstadoRegistro Estado { get; private set; }

    protected Ubicacion() { }

    private Ubicacion(int sedeId, string nombre, string? descripcion)
    {
        SedeId = sedeId;
        Nombre = nombre;
        Descripcion = descripcion;
        Estado = EstadoRegistro.Activo;
    }

    public static Ubicacion Crear(int sedeId, string nombre, string? descripcion = null)
    {
        ValidarSedeId(sedeId);
        ValidarNombre(nombre);
        ValidarDescripcion(descripcion);

        return new Ubicacion(sedeId, nombre.Trim(), descripcion?.Trim());
    }

    public void ActualizarDatos(int sedeId, string nombre, string? descripcion)
    {
        ValidarSedeId(sedeId);
        ValidarNombre(nombre);
        ValidarDescripcion(descripcion);

        SedeId = sedeId;
        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim();
    }

    public void Activar() => Estado = EstadoRegistro.Activo;

    public void Desactivar() => Estado = EstadoRegistro.Inactivo;

    private static void ValidarSedeId(int sedeId)
    {
        if (sedeId <= 0)
            throw new ArgumentException("La sede es obligatoria.", nameof(sedeId));
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la ubicación es obligatorio.", nameof(nombre));
        if (nombre.Length > 100)
            throw new ArgumentException("El nombre de la ubicación no puede exceder 100 caracteres.", nameof(nombre));
    }

    private static void ValidarDescripcion(string? descripcion)
    {
        if (descripcion is not null && descripcion.Trim().Length > 200)
            throw new ArgumentException("La descripción de la ubicación no puede exceder 200 caracteres.", nameof(descripcion));
    }
}
