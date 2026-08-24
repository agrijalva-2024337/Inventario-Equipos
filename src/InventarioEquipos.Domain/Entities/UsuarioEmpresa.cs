using InventarioEquipos.Domain.Common;
using InventarioEquipos.Domain.Enums;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Relación N a N entre Usuario y Empresa: define el rol de un usuario en
/// una empresa concreta, si esa empresa es la predeterminada para él al
/// iniciar sesión (soporte multiempresa) y cuándo se hizo esa asignación.
/// Depende de Usuario y Empresa, por lo que solo puede mergearse después
/// de que ambas estén en main. Columnas según el diagrama: id_usuario,
/// id_empresa, rol, empresa_predeterminada, fecha_asignacion.
/// Las FKs se nombran IdUsuario / IdEmpresa para que coincidan con
/// id_usuario / id_empresa al pasar a snake_case en el DbContext.
/// </summary>
public class UsuarioEmpresa : EntityBase
{
    public int IdUsuario { get; private set; }
    public Usuario? Usuario { get; private set; }

    public int IdEmpresa { get; private set; }
    public Empresa? Empresa { get; private set; }

    public RolUsuarioEmpresa Rol { get; private set; }

    public bool EmpresaPredeterminada { get; private set; }

    public DateTime FechaAsignacion { get; private set; }

    protected UsuarioEmpresa() { }

    private UsuarioEmpresa(int idUsuario, int idEmpresa, RolUsuarioEmpresa rol, bool empresaPredeterminada)
    {
        IdUsuario = idUsuario;
        IdEmpresa = idEmpresa;
        Rol = rol;
        EmpresaPredeterminada = empresaPredeterminada;
        FechaAsignacion = DateTime.UtcNow;
    }

    public static UsuarioEmpresa Crear(
        int idUsuario,
        int idEmpresa,
        RolUsuarioEmpresa rol,
        bool empresaPredeterminada = false)
    {
        if (idUsuario <= 0)
            throw new ArgumentException("El usuario es obligatorio.", nameof(idUsuario));
        if (idEmpresa <= 0)
            throw new ArgumentException("La empresa es obligatoria.", nameof(idEmpresa));

        return new UsuarioEmpresa(idUsuario, idEmpresa, rol, empresaPredeterminada);
    }

    public void CambiarRol(RolUsuarioEmpresa nuevoRol) => Rol = nuevoRol;

    /// <summary>
    /// Marca esta empresa como predeterminada para el usuario. La regla de
    /// "solo una empresa predeterminada por usuario" cruza varias filas
    /// (varios UsuarioEmpresa del mismo usuario), así que se resuelve a
    /// nivel de servicio de aplicación (desmarcando las demás filas del
    /// mismo IdUsuario), no dentro de esta entidad aislada.
    /// </summary>
    public void MarcarComoPredeterminada() => EmpresaPredeterminada = true;

    public void QuitarPredeterminada() => EmpresaPredeterminada = false;
}
