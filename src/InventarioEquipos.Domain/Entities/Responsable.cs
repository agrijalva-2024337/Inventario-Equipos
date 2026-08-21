using System.Text.RegularExpressions;
using InventarioEquipos.Domain.Common;
using InventarioEquipos.Domain.Enums;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Responsable de activos, ligado a una empresa y un área. Depende de
/// Empresa y Area. IdUsuario es opcional: un responsable puede no tener
/// cuenta de acceso al sistema. Columnas según el diagrama: id_empresa,
/// id_area, id_usuario (nullable), nombre_completo, cargo, correo,
/// telefono, estado.
/// </summary>
public class Responsable : EntityBase
{
    private static readonly Regex CorreoRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public int IdEmpresa { get; private set; }
    public Empresa? Empresa { get; private set; }

    public int IdArea { get; private set; }
    public Area? Area { get; private set; }

    public int? IdUsuario { get; private set; }
    public Usuario? Usuario { get; private set; }

    public string NombreCompleto { get; private set; } = default!;
    public string? Cargo { get; private set; }
    public string? Correo { get; private set; }
    public string? Telefono { get; private set; }
    public EstadoRegistro Estado { get; private set; }

    protected Responsable() { }

    private Responsable(
        int idEmpresa,
        int idArea,
        int? idUsuario,
        string nombreCompleto,
        string? cargo,
        string? correo,
        string? telefono)
    {
        IdEmpresa = idEmpresa;
        IdArea = idArea;
        IdUsuario = idUsuario;
        NombreCompleto = nombreCompleto;
        Cargo = cargo;
        Correo = correo;
        Telefono = telefono;
        Estado = EstadoRegistro.Activo;
    }

    public static Responsable Crear(
        int idEmpresa,
        int idArea,
        string nombreCompleto,
        int? idUsuario = null,
        string? cargo = null,
        string? correo = null,
        string? telefono = null)
    {
        ValidarIdEmpresa(idEmpresa);
        ValidarIdArea(idArea);
        ValidarIdUsuario(idUsuario);
        ValidarNombreCompleto(nombreCompleto);
        ValidarCargo(cargo);
        correo = ValidarCorreoOpcional(correo);
        ValidarTelefono(telefono);

        return new Responsable(
            idEmpresa,
            idArea,
            idUsuario,
            nombreCompleto.Trim(),
            cargo?.Trim(),
            correo,
            telefono?.Trim());
    }

    public void ActualizarDatos(
        int idEmpresa,
        int idArea,
        string nombreCompleto,
        int? idUsuario,
        string? cargo,
        string? correo,
        string? telefono)
    {
        ValidarIdEmpresa(idEmpresa);
        ValidarIdArea(idArea);
        ValidarIdUsuario(idUsuario);
        ValidarNombreCompleto(nombreCompleto);
        ValidarCargo(cargo);
        correo = ValidarCorreoOpcional(correo);
        ValidarTelefono(telefono);

        IdEmpresa = idEmpresa;
        IdArea = idArea;
        IdUsuario = idUsuario;
        NombreCompleto = nombreCompleto.Trim();
        Cargo = cargo?.Trim();
        Correo = correo;
        Telefono = telefono?.Trim();
    }

    public void Activar() => Estado = EstadoRegistro.Activo;

    public void Desactivar() => Estado = EstadoRegistro.Inactivo;

    private static void ValidarIdEmpresa(int idEmpresa)
    {
        if (idEmpresa <= 0)
            throw new ArgumentException("La empresa es obligatoria.", nameof(idEmpresa));
    }

    private static void ValidarIdArea(int idArea)
    {
        if (idArea <= 0)
            throw new ArgumentException("El área es obligatoria.", nameof(idArea));
    }

    private static void ValidarIdUsuario(int? idUsuario)
    {
        if (idUsuario is <= 0)
            throw new ArgumentException("El usuario, si se informa, debe ser mayor a 0.", nameof(idUsuario));
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
