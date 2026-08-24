using InventarioEquipos.Domain.Common;
using InventarioEquipos.Domain.Enums;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Sede física de una empresa en un país. Depende de Empresa y Pais.
/// Columnas según el diagrama: id_empresa, id_pais, nombre, direccion
/// (VARCHAR 100 NOT NULL), ciudad (VARCHAR 100 NOT NULL), estado.
/// Las FKs se nombran IdEmpresa / IdPais para que el orden de palabras
/// coincida con id_empresa / id_pais al pasar a snake_case en el DbContext.
/// </summary>
public class Sede : EntityBase
{
    public int IdEmpresa { get; private set; }
    public Empresa? Empresa { get; private set; }

    public int IdPais { get; private set; }
    public Pais? Pais { get; private set; }

    public string Nombre { get; private set; } = default!;
    public string Direccion { get; private set; } = default!;
    public string Ciudad { get; private set; } = default!;
    public EstadoRegistro Estado { get; private set; }

    protected Sede() { }

    private Sede(
        int idEmpresa,
        int idPais,
        string nombre,
        string direccion,
        string ciudad)
    {
        IdEmpresa = idEmpresa;
        IdPais = idPais;
        Nombre = nombre;
        Direccion = direccion;
        Ciudad = ciudad;
        Estado = EstadoRegistro.Activo;
    }

    public static Sede Crear(
        int idEmpresa,
        int idPais,
        string nombre,
        string direccion,
        string ciudad)
    {
        ValidarIdEmpresa(idEmpresa);
        ValidarIdPais(idPais);
        ValidarNombre(nombre);
        ValidarDireccion(direccion);
        ValidarCiudad(ciudad);

        return new Sede(idEmpresa, idPais, nombre.Trim(), direccion.Trim(), ciudad.Trim());
    }

    public void ActualizarDatos(
        int idEmpresa,
        int idPais,
        string nombre,
        string direccion,
        string ciudad)
    {
        ValidarIdEmpresa(idEmpresa);
        ValidarIdPais(idPais);
        ValidarNombre(nombre);
        ValidarDireccion(direccion);
        ValidarCiudad(ciudad);

        IdEmpresa = idEmpresa;
        IdPais = idPais;
        Nombre = nombre.Trim();
        Direccion = direccion.Trim();
        Ciudad = ciudad.Trim();
    }

    public void Activar() => Estado = EstadoRegistro.Activo;

    public void Desactivar() => Estado = EstadoRegistro.Inactivo;

    private static void ValidarIdEmpresa(int idEmpresa)
    {
        if (idEmpresa <= 0)
            throw new ArgumentException("La empresa es obligatoria.", nameof(idEmpresa));
    }

    private static void ValidarIdPais(int idPais)
    {
        if (idPais <= 0)
            throw new ArgumentException("El país es obligatorio.", nameof(idPais));
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la sede es obligatorio.", nameof(nombre));
        if (nombre.Length > 100)
            throw new ArgumentException("El nombre de la sede no puede exceder 100 caracteres.", nameof(nombre));
    }

    private static void ValidarDireccion(string direccion)
    {
        if (string.IsNullOrWhiteSpace(direccion))
            throw new ArgumentException("La dirección de la sede es obligatoria.", nameof(direccion));
        if (direccion.Trim().Length > 100)
            throw new ArgumentException("La dirección de la sede no puede exceder 100 caracteres.", nameof(direccion));
    }

    private static void ValidarCiudad(string ciudad)
    {
        if (string.IsNullOrWhiteSpace(ciudad))
            throw new ArgumentException("La ciudad de la sede es obligatoria.", nameof(ciudad));
        if (ciudad.Trim().Length > 100)
            throw new ArgumentException("La ciudad de la sede no puede exceder 100 caracteres.", nameof(ciudad));
    }
}
