using InventarioEquipos.Domain.Common;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Catálogo de estados posibles de un activo (Disponible, Asignado, En
/// mantenimiento, etc.) por empresa. Depende de Empresa. Columnas según
/// el diagrama: id_empresa, nombre, descripcion. El PK del diagrama es
/// id_estado (no id_estado_activo). Esta tabla no tiene columna estado
/// propia, por eso no usa Activar/Desactivar ni el enum EstadoRegistro.
/// </summary>
public class EstadoActivo : EntityBase
{
    public int IdEmpresa { get; private set; }
    public Empresa? Empresa { get; private set; }

    public string Nombre { get; private set; } = default!;
    public string? Descripcion { get; private set; }

    protected EstadoActivo() { }

    private EstadoActivo(int idEmpresa, string nombre, string? descripcion)
    {
        IdEmpresa = idEmpresa;
        Nombre = nombre;
        Descripcion = descripcion;
    }

    public static EstadoActivo Crear(int idEmpresa, string nombre, string? descripcion = null)
    {
        ValidarIdEmpresa(idEmpresa);
        ValidarNombre(nombre);
        ValidarDescripcion(descripcion);

        return new EstadoActivo(idEmpresa, nombre.Trim(), descripcion?.Trim());
    }

    public void ActualizarDatos(int idEmpresa, string nombre, string? descripcion)
    {
        ValidarIdEmpresa(idEmpresa);
        ValidarNombre(nombre);
        ValidarDescripcion(descripcion);

        IdEmpresa = idEmpresa;
        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim();
    }

    private static void ValidarIdEmpresa(int idEmpresa)
    {
        if (idEmpresa <= 0)
            throw new ArgumentException("La empresa es obligatoria.", nameof(idEmpresa));
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
