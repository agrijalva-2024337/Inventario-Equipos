namespace InventarioEquipos.Domain.Enums;

/// <summary>
/// Tipo de operación registrada en el historial de cambios (auditoría de negocio).
/// </summary>
public enum TipoOperacionHistorial
{
    Creacion = 1,
    Actualizacion = 2,
    Eliminacion = 3,
    Restauracion = 4,
    CambioEstado = 5
}
