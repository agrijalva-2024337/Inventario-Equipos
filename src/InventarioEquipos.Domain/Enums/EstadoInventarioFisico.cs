namespace InventarioEquipos.Domain.Enums;

/// <summary>
/// Estado del ciclo de vida de un inventario físico. Se persiste como texto
/// (VARCHAR 20) vía conversión de EF Core, igual que EstadoMantenimiento.
/// </summary>
public enum EstadoInventarioFisico
{
    Abierto = 1,
    Cerrado = 2,
    Cancelado = 3
}
