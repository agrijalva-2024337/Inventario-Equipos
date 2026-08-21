using InventarioEquipos.Domain.Common;
using InventarioEquipos.Domain.Enums;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Sede física de una empresa en un país. Depende de Empresa y Pais, por
/// lo que solo puede mergearse después de que ambas estén en main.
/// Columnas según el diagrama: id_empresa, id_pais, nombre, direccion,
/// ciudad, estado. (El diagrama no incluye fecha_creacion para esta tabla.)
/// </summary>
public class Sede : EntityBase
{
    public int EmpresaId { get; private set; }
    public Empresa? Empresa { get; private set; }

    public int PaisId { get; private set; }
    public Pais? Pais { get; private set; }

    public string Nombre { get; private set; } = default!;
    public string? Direccion { get; private set; }
    public string? Ciudad { get; private set; }
    public EstadoRegistro Estado { get; private set; }

    protected Sede() { }

    private Sede(
        int empresaId,
        int paisId,
        string nombre,
        string? direccion,
        string? ciudad)
    {
        EmpresaId = empresaId;
        PaisId = paisId;
        Nombre = nombre;
        Direccion = direccion;
        Ciudad = ciudad;
        Estado = EstadoRegistro.Activo;
    }

    public static Sede Crear(
        int empresaId,
        int paisId,
        string nombre,
        string? direccion = null,
        string? ciudad = null)
    {
        ValidarEmpresaId(empresaId);
        ValidarPaisId(paisId);
        ValidarNombre(nombre);
        ValidarDireccion(direccion);
        ValidarCiudad(ciudad);

        return new Sede(empresaId, paisId, nombre.Trim(), direccion?.Trim(), ciudad?.Trim());
    }

    public void ActualizarDatos(
        int empresaId,
        int paisId,
        string nombre,
        string? direccion,
        string? ciudad)
    {
        ValidarEmpresaId(empresaId);
        ValidarPaisId(paisId);
        ValidarNombre(nombre);
        ValidarDireccion(direccion);
        ValidarCiudad(ciudad);

        EmpresaId = empresaId;
        PaisId = paisId;
        Nombre = nombre.Trim();
        Direccion = direccion?.Trim();
        Ciudad = ciudad?.Trim();
    }

    public void Activar() => Estado = EstadoRegistro.Activo;

    public void Desactivar() => Estado = EstadoRegistro.Inactivo;

    private static void ValidarEmpresaId(int empresaId)
    {
        if (empresaId <= 0)
            throw new ArgumentException("La empresa es obligatoria.", nameof(empresaId));
    }

    private static void ValidarPaisId(int paisId)
    {
        if (paisId <= 0)
            throw new ArgumentException("El país es obligatorio.", nameof(paisId));
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la sede es obligatorio.", nameof(nombre));
        if (nombre.Length > 100)
            throw new ArgumentException("El nombre de la sede no puede exceder 100 caracteres.", nameof(nombre));
    }

    private static void ValidarDireccion(string? direccion)
    {
        if (direccion is not null && direccion.Trim().Length > 200)
            throw new ArgumentException("La dirección de la sede no puede exceder 200 caracteres.", nameof(direccion));
    }

    private static void ValidarCiudad(string? ciudad)
    {
        if (ciudad is not null && ciudad.Trim().Length > 100)
            throw new ArgumentException("La ciudad de la sede no puede exceder 100 caracteres.", nameof(ciudad));
    }
}
