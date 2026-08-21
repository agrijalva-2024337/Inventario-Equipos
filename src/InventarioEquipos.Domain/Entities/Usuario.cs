using System.Text.RegularExpressions;
using InventarioEquipos.Domain.Common;
using InventarioEquipos.Domain.Enums;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Usuario del sistema. No depende de Empresa (la relación multiempresa
/// vive en UsuarioEmpresa). Columnas según el diagrama: nombre_completo,
/// correo, usuario_login, password_hash, estado, fecha_creacion.
/// El hash de la contraseña se recibe ya calculado (hashing es
/// responsabilidad de Application/Infrastructure, no del dominio).
/// </summary>
public class Usuario : EntityBase
{
    private static readonly Regex CorreoRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public string NombreCompleto { get; private set; } = default!;
    public string Correo { get; private set; } = default!;
    public string UsuarioLogin { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public EstadoRegistro Estado { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    protected Usuario() { }

    private Usuario(
        string nombreCompleto,
        string correo,
        string usuarioLogin,
        string passwordHash)
    {
        NombreCompleto = nombreCompleto;
        Correo = correo;
        UsuarioLogin = usuarioLogin;
        PasswordHash = passwordHash;
        Estado = EstadoRegistro.Activo;
        FechaCreacion = DateTime.UtcNow;
    }

    public static Usuario Crear(
        string nombreCompleto,
        string correo,
        string usuarioLogin,
        string passwordHash)
    {
        ValidarNombreCompleto(nombreCompleto);
        ValidarCorreo(correo);
        ValidarUsuarioLogin(usuarioLogin);
        ValidarPasswordHash(passwordHash);

        return new Usuario(
            nombreCompleto.Trim(),
            correo.Trim().ToLowerInvariant(),
            usuarioLogin.Trim().ToLowerInvariant(),
            passwordHash);
    }

    public void ActualizarDatosPersonales(string nombreCompleto, string correo)
    {
        ValidarNombreCompleto(nombreCompleto);
        ValidarCorreo(correo);

        NombreCompleto = nombreCompleto.Trim();
        Correo = correo.Trim().ToLowerInvariant();
    }

    public void CambiarPasswordHash(string nuevoPasswordHash)
    {
        ValidarPasswordHash(nuevoPasswordHash);
        PasswordHash = nuevoPasswordHash;
    }

    public void Activar() => Estado = EstadoRegistro.Activo;

    public void Desactivar() => Estado = EstadoRegistro.Inactivo;

    private static void ValidarNombreCompleto(string nombreCompleto)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto))
            throw new ArgumentException("El nombre completo es obligatorio.", nameof(nombreCompleto));
        if (nombreCompleto.Length > 150)
            throw new ArgumentException("El nombre completo no puede exceder 150 caracteres.", nameof(nombreCompleto));
    }

    private static void ValidarCorreo(string correo)
    {
        if (string.IsNullOrWhiteSpace(correo) || correo.Length > 150 || !CorreoRegex.IsMatch(correo.Trim()))
            throw new ArgumentException("El correo no tiene un formato válido.", nameof(correo));
    }

    private static void ValidarUsuarioLogin(string usuarioLogin)
    {
        if (string.IsNullOrWhiteSpace(usuarioLogin))
            throw new ArgumentException("El usuario de login es obligatorio.", nameof(usuarioLogin));
        if (usuarioLogin.Trim().Length < 3 || usuarioLogin.Trim().Length > 50)
            throw new ArgumentException("El usuario de login debe tener entre 3 y 50 caracteres.", nameof(usuarioLogin));
    }

    private static void ValidarPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("El hash de la contraseña es obligatorio (no se aceptan contraseñas en texto plano en el dominio).", nameof(passwordHash));
        if (passwordHash.Length > 255)
            throw new ArgumentException("El hash de la contraseña no puede exceder 255 caracteres.", nameof(passwordHash));
    }
}
