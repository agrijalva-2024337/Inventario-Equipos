using InventarioEquipos.Domain.Common;
using InventarioEquipos.Domain.Enums;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Catálogo de categorías de activos por empresa. Depende de Empresa.
/// Columnas según el diagrama: id_empresa, nombre, descripcion, estado.
/// </summary>
public class CategoriaActivo : EntityBase
{
    public int EmpresaId { get; private set; }
    public Empresa? Empresa { get; private set; }

    public string Nombre { get; private set; } = default!;
    public string? Descripcion { get; private set; }
    public EstadoRegistro Estado { get; private set; }

    protected CategoriaActivo() { }

    private CategoriaActivo(int empresaId, string nombre, string? descripcion)
    {
        EmpresaId = empresaId;
        Nombre = nombre;
        Descripcion = descripcion;
        Estado = EstadoRegistro.Activo;
    }

    public static CategoriaActivo Crear(int empresaId, string nombre, string? descripcion = null)
    {
        ValidarEmpresaId(empresaId);
        ValidarNombre(nombre);
        ValidarDescripcion(descripcion);

        return new CategoriaActivo(empresaId, nombre.Trim(), descripcion?.Trim());
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

    public void Activar() => Estado = EstadoRegistro.Activo;

    public void Desactivar() => Estado = EstadoRegistro.Inactivo;

    private static void ValidarEmpresaId(int empresaId)
    {
        if (empresaId <= 0)
            throw new ArgumentException("La empresa es obligatoria.", nameof(empresaId));
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la categoría es obligatorio.", nameof(nombre));
        if (nombre.Length > 100)
            throw new ArgumentException("El nombre de la categoría no puede exceder 100 caracteres.", nameof(nombre));
    }

    private static void ValidarDescripcion(string? descripcion)
    {
        if (descripcion is not null && descripcion.Trim().Length > 200)
            throw new ArgumentException("La descripción de la categoría no puede exceder 200 caracteres.", nameof(descripcion));
    }
}
