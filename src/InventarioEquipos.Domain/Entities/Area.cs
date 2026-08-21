using InventarioEquipos.Domain.Common;
using InventarioEquipos.Domain.Enums;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Área o departamento de una empresa. Depende de Empresa. Columnas según
/// el diagrama: id_empresa, nombre, descripcion, estado.
/// </summary>
public class Area : EntityBase
{
    public int EmpresaId { get; private set; }
    public Empresa? Empresa { get; private set; }

    public string Nombre { get; private set; } = default!;
    public string? Descripcion { get; private set; }
    public EstadoRegistro Estado { get; private set; }

    protected Area() { }

    private Area(int empresaId, string nombre, string? descripcion)
    {
        EmpresaId = empresaId;
        Nombre = nombre;
        Descripcion = descripcion;
        Estado = EstadoRegistro.Activo;
    }

    public static Area Crear(int empresaId, string nombre, string? descripcion = null)
    {
        ValidarEmpresaId(empresaId);
        ValidarNombre(nombre);
        ValidarDescripcion(descripcion);

        return new Area(empresaId, nombre.Trim(), descripcion?.Trim());
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
            throw new ArgumentException("El nombre del área es obligatorio.", nameof(nombre));
        if (nombre.Length > 100)
            throw new ArgumentException("El nombre del área no puede exceder 100 caracteres.", nameof(nombre));
    }

    private static void ValidarDescripcion(string? descripcion)
    {
        if (descripcion is not null && descripcion.Trim().Length > 200)
            throw new ArgumentException("La descripción del área no puede exceder 200 caracteres.", nameof(descripcion));
    }
}
