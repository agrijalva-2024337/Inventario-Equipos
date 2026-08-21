using System.Text.RegularExpressions;
using InventarioEquipos.Domain.Common;
using InventarioEquipos.Domain.Enums;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Proveedor de una empresa. Depende de Empresa. Columnas según el
/// diagrama: id_empresa, nombre, nit, contacto, telefono, correo, estado.
/// </summary>
public class Proveedor : EntityBase
{
    private static readonly Regex CorreoRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public int EmpresaId { get; private set; }
    public Empresa? Empresa { get; private set; }

    public string Nombre { get; private set; } = default!;
    public string? Nit { get; private set; }
    public string? Contacto { get; private set; }
    public string? Telefono { get; private set; }
    public string? Correo { get; private set; }
    public EstadoRegistro Estado { get; private set; }

    protected Proveedor() { }

    private Proveedor(
        int empresaId,
        string nombre,
        string? nit,
        string? contacto,
        string? telefono,
        string? correo)
    {
        EmpresaId = empresaId;
        Nombre = nombre;
        Nit = nit;
        Contacto = contacto;
        Telefono = telefono;
        Correo = correo;
        Estado = EstadoRegistro.Activo;
    }

    public static Proveedor Crear(
        int empresaId,
        string nombre,
        string? nit = null,
        string? contacto = null,
        string? telefono = null,
        string? correo = null)
    {
        ValidarEmpresaId(empresaId);
        ValidarNombre(nombre);
        ValidarNit(nit);
        ValidarContacto(contacto);
        ValidarTelefono(telefono);
        correo = ValidarCorreoOpcional(correo);

        return new Proveedor(
            empresaId,
            nombre.Trim(),
            nit?.Trim(),
            contacto?.Trim(),
            telefono?.Trim(),
            correo);
    }

    public void ActualizarDatos(
        int empresaId,
        string nombre,
        string? nit,
        string? contacto,
        string? telefono,
        string? correo)
    {
        ValidarEmpresaId(empresaId);
        ValidarNombre(nombre);
        ValidarNit(nit);
        ValidarContacto(contacto);
        ValidarTelefono(telefono);
        correo = ValidarCorreoOpcional(correo);

        EmpresaId = empresaId;
        Nombre = nombre.Trim();
        Nit = nit?.Trim();
        Contacto = contacto?.Trim();
        Telefono = telefono?.Trim();
        Correo = correo;
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
            throw new ArgumentException("El nombre del proveedor es obligatorio.", nameof(nombre));
        if (nombre.Length > 150)
            throw new ArgumentException("El nombre del proveedor no puede exceder 150 caracteres.", nameof(nombre));
    }

    private static void ValidarNit(string? nit)
    {
        if (nit is not null && nit.Trim().Length > 50)
            throw new ArgumentException("El NIT no puede exceder 50 caracteres.", nameof(nit));
    }

    private static void ValidarContacto(string? contacto)
    {
        if (contacto is not null && contacto.Trim().Length > 100)
            throw new ArgumentException("El contacto no puede exceder 100 caracteres.", nameof(contacto));
    }

    private static void ValidarTelefono(string? telefono)
    {
        if (telefono is not null && telefono.Trim().Length > 30)
            throw new ArgumentException("El teléfono no puede exceder 30 caracteres.", nameof(telefono));
    }

    private static string? ValidarCorreoOpcional(string? correo)
    {
        if (string.IsNullOrWhiteSpace(correo))
            return null;

        correo = correo.Trim().ToLowerInvariant();
        if (correo.Length > 150 || !CorreoRegex.IsMatch(correo))
            throw new ArgumentException("El correo no tiene un formato válido.", nameof(correo));

        return correo;
    }
}
