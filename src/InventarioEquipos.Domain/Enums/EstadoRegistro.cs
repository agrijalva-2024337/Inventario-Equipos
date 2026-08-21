namespace InventarioEquipos.Domain.Enums;

/// <summary>
/// Estado genérico usado por Empresa, Pais y Usuario. Es independiente del
/// soft delete de EntityBase (EstaEliminado): "Inactivo" significa que el
/// registro existe pero está deshabilitado para operar (por decisión de
/// negocio), no que se haya borrado.
/// </summary>
public enum EstadoRegistro
{
    Activo = 1,
    Inactivo = 2
}