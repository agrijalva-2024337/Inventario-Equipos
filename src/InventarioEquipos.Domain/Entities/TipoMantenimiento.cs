using InventarioEquipos.Domain.Common;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Catálogo de tipos de mantenimiento (preventivo, correctivo, etc.) por
/// empresa. Depende de Empresa. Columnas según el diagrama: id_empresa,
/// nombre, descripcion. El diagrama no incluye columna estado, por eso
/// no usa Activar/Desactivar.
/// </summary>
public class TipoMantenimiento : EntityBase
{
    public int EmpresaId { get; private set; }
    public Empresa? Empresa { get; private set; }

    public string Nombre { get; private set; } = default!;
    public string? Descripcion { get; private set; }

    protected TipoMantenimiento() { }

    private TipoMantenimiento(int empresaId, string nombre, string? descripcion)
    {
        EmpresaId = empresaId;
        Nombre = nombre;
        Descripcion = descripcion;
    }

    public static TipoMantenimiento Crear(int empresaId, string nombre, string? descripcion = null)
    {
        ValidarEmpresaId(empresaId);
        ValidarNombre(nombre);
        ValidarDescripcion(descripcion);

        return new TipoMantenimiento(empresaId, nombre.Trim(), descripcion?.Trim());
    }

    public void ActualizarDatos(int empresaId, string nombre, string? descripcion)
    {
        ValidarEmpresaId(empresaId);
        ValidarNombre(nombre);
        ValidarDescripcion(descripcion);

        EmpresaId = empresaId;
        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim();
    }

    private static void ValidarEmpresaId(int empresaId)
    {
        if (empresaId <= 0)
            throw new ArgumentException("La empresa es obligatoria.", nameof(empresaId));
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del tipo de mantenimiento es obligatorio.", nameof(nombre));
        if (nombre.Length > 50)
            throw new ArgumentException("El nombre del tipo de mantenimiento no puede exceder 50 caracteres.", nameof(nombre));
    }

    private static void ValidarDescripcion(string? descripcion)
    {
        if (descripcion is not null && descripcion.Trim().Length > 150)
            throw new ArgumentException("La descripción del tipo de mantenimiento no puede exceder 150 caracteres.", nameof(descripcion));
    }
}
