using InventarioEquipos.Domain.Common;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Catálogo de estados posibles de un activo (Disponible, Asignado, En
/// mantenimiento, etc.) por empresa. Depende de Empresa. Columnas según
/// el diagrama: id_empresa, nombre, descripcion. Esta tabla no tiene
/// columna estado propia: ella misma representa esos estados, por eso
/// no usa Activar/Desactivar ni el enum EstadoRegistro.
/// </summary>
public class EstadoActivo : EntityBase
{
    public int EmpresaId { get; private set; }
    public Empresa? Empresa { get; private set; }

    public string Nombre { get; private set; } = default!;
    public string? Descripcion { get; private set; }

    protected EstadoActivo() { }

    private EstadoActivo(int empresaId, string nombre, string? descripcion)
    {
        EmpresaId = empresaId;
        Nombre = nombre;
        Descripcion = descripcion;
    }

    public static EstadoActivo Crear(int empresaId, string nombre, string? descripcion = null)
    {
        ValidarEmpresaId(empresaId);
        ValidarNombre(nombre);
        ValidarDescripcion(descripcion);

        return new EstadoActivo(empresaId, nombre.Trim(), descripcion?.Trim());
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
            throw new ArgumentException("El nombre del estado de activo es obligatorio.", nameof(nombre));
        if (nombre.Length > 50)
            throw new ArgumentException("El nombre del estado de activo no puede exceder 50 caracteres.", nameof(nombre));
    }

    private static void ValidarDescripcion(string? descripcion)
    {
        if (descripcion is not null && descripcion.Trim().Length > 150)
            throw new ArgumentException("La descripción del estado de activo no puede exceder 150 caracteres.", nameof(descripcion));
    }
}
