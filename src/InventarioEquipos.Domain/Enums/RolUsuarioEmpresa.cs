namespace InventarioEquipos.Domain.Enums;

/// <summary>
/// Rol que un Usuario tiene dentro de una Empresa específica (relación
/// multiempresa: el mismo usuario puede tener roles distintos en distintas
/// empresas). Ajustar/ampliar según el catálogo real de roles del negocio.
/// </summary>
public enum RolUsuarioEmpresa
{
    Administrador = 1,
    Supervisor = 2,
    Operador = 3,
    Consulta = 4
}
