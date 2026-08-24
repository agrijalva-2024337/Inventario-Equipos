namespace InventarioEquipos.Domain.Enums;

/// <summary>
/// Estado del ciclo de vida de un mantenimiento. Se persiste como texto
/// (VARCHAR 30) vía conversión de EF Core, igual que EstadoRegistro.
/// </summary>
public enum EstadoMantenimiento
{
    Programado = 1,
    EnProceso = 2,
    Completado = 3,
    Cancelado = 4
}
