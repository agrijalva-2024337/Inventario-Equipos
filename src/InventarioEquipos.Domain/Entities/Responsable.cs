using System.Text.RegularExpressions;
using InventarioEquipos.Domain.Common;
using InventarioEquipos.Domain.Enums;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Responsable de activos, ligado a una empresa y un área. Depende de
/// Empresa y Area. UsuarioId es opcional: un responsable puede no tener
/// cuenta de acceso al sistema. Columnas según el diagrama: id_empresa,
/// id_area, id_usuario (nullable), nombre_completo, cargo, correo,
/// telefono, estado.
/// </summary>
public class Responsable : EntityBase
{
    private static readonly Regex CorreoRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public int EmpresaId { get; private set; }
    public Empresa? Empresa { get; private set; }

    public int AreaId { get; private set; }
    public Area? Area { get; private set; }

    public int? UsuarioId { get; private set; }
    public Usuario? Usuario { get; private set; }

    public string NombreCompleto { get; private set; } = default!;
    public string? Cargo { get; private set; }
    public string? Correo { get; private set; }
    public string? Telefono { get; private set; }
    public EstadoRegistro Estado { get; private set; }

    protected Responsable() { }

    private Responsable(
        int empresaId,
        int areaId,
        int? usuarioId,
        string nombreCompleto,
        string? cargo,
        string? correo,
        string? telefono)
    {
        EmpresaId = empresaId;
        AreaId = areaId;
        UsuarioId = usuarioId;
        NombreCompleto = nombreCompleto;
        Cargo = cargo;
        Correo = correo;
        Telefono = telefono;
        Estado = EstadoRegistro.Activo;
    }

    public static Responsable Crear(
        int empresaId,
        int areaId,
        string nombreCompleto,
        int? usuarioId = null,
        string? cargo = null,
        string? correo = null,
        string? telefono = null)
    {
        ValidarEmpresaId(empresaId);
        ValidarAreaId(areaId);
        ValidarUsuarioId(usuarioId);
        ValidarNombreCompleto(nombreCompleto);
        ValidarCargo(cargo);
        correo = ValidarCorreoOpcional(correo);
        ValidarTelefono(telefono);

        return new Responsable(
            empresaId,
            areaId,
            usuarioId,
            nombreCompleto.Trim(),
            cargo?.Trim(),
            correo,
            telefono?.Trim());
    }

    public void ActualizarDatos(
        int empresaId,
        int areaId,
        string nombreCompleto,
        int? usuarioId,
        string? cargo,
        string? correo,
        string? telefono)
    {
        ValidarEmpresaId(empresaId);
        ValidarAreaId(areaId);
        ValidarUsuarioId(usuarioId);
        ValidarNombreCompleto(nombreCompleto);
        ValidarCargo(cargo);
        correo = ValidarCorreoOpcional(correo);
        ValidarTelefono(telefono);

        EmpresaId = empresaId;
        AreaId = areaId;
        UsuarioId = usuarioId;
        NombreCompleto = nombreCompleto.Trim();
        Cargo = cargo?.Trim();
        Correo = correo;
        Telefono = telefono?.Trim();
    }

    public void Activar() => Estado = EstadoRegistro.Activo;

    public void Desactivar() => Estado = EstadoRegistro.Inactivo;

    private static void ValidarEmpresaId(int empresaId)
    {
        if (empresaId <= 0)
            throw new ArgumentException("La empresa es obligatoria.", nameof(empresaId));
    }

    private static void ValidarAreaId(int areaId)
    {
        if (areaId <= 0)
            throw new ArgumentException("El área es obligatoria.", nameof(areaId));
    }

    private static void ValidarUsuarioId(int? usuarioId)
    {
        if (usuarioId is <= 0)
            throw new ArgumentException("El usuario, si se informa, debe ser mayor a 0.", nameof(usuarioId));
    }

    private static void ValidarNombreCompleto(string nombreCompleto)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto))
            throw new ArgumentException("El nombre completo del responsable es obligatorio.", nameof(nombreCompleto));
        if (nombreCompleto.Length > 150)
            throw new ArgumentException("El nombre completo no puede exceder 150 caracteres.", nameof(nombreCompleto));
    }

    private static void ValidarCargo(string? cargo)
    {
        if (cargo is not null && cargo.Trim().Length > 100)
            throw new ArgumentException("El cargo no puede exceder 100 caracteres.", nameof(cargo));
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

    private static void ValidarTelefono(string? telefono)
    {
        if (telefono is not null && telefono.Trim().Length > 30)
            throw new ArgumentException("El teléfono no puede exceder 30 caracteres.", nameof(telefono));
    }
}
