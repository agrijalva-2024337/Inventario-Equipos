using InventarioEquipos.Domain.Common;
using InventarioEquipos.Domain.Enums;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Intervención de mantenimiento (preventivo, correctivo, etc.) sobre un
/// activo. Depende de Activo y TipoMantenimiento (obligatorios) y de
/// Proveedor (opcional: el trabajo puede hacerlo personal interno).
/// Columnas según el diagrama: id_activo, id_tipo_mantenimiento,
/// id_proveedor, fecha_programada, fecha_realizado, responsable,
/// descripcion_problema, trabajo_realizado, costo, numero_factura,
/// estado_mantenimiento. El PK del diagrama es id_mantenimiento.
/// Las FKs se nombran IdActivo / IdTipoMantenimiento / IdProveedor
/// para que el orden de palabras coincida con id_activo /
/// id_tipo_mantenimiento / id_proveedor al pasar a snake_case en
/// el DbContext.
///
/// El diagrama no incluye columna estado (Activo/Inactivo), por eso no
/// usa Activar/Desactivar; el ciclo de vida lo expresa
/// EstadoMantenimiento (Programado, EnProceso, Completado, Cancelado).
///
/// Reglas de negocio del DERCAS que NO se validan aquí porque cruzan
/// varias filas / varias entidades (van en Application):
/// - "Un activo dado de baja no podrá asignarse, trasladarse ni enviarse
///   a mantenimiento": requiere conocer el catálogo EstadoActivo vigente.
/// - El tipo de mantenimiento y el proveedor (si se informa) deben
///   existir, estar vigentes y pertenecer a la misma empresa del activo.
/// </summary>
public class Mantenimiento : EntityBase
{
    private const decimal CostoMaximo = 9_999_999_999.99m;

    public int IdActivo { get; private set; }
    public Activo? Activo { get; private set; }

    public int IdTipoMantenimiento { get; private set; }
    public TipoMantenimiento? TipoMantenimiento { get; private set; }

    public int? IdProveedor { get; private set; }
    public Proveedor? Proveedor { get; private set; }

    public DateTime FechaProgramada { get; private set; }
    public DateTime? FechaRealizado { get; private set; }
    public string? Responsable { get; private set; }
    public string? DescripcionProblema { get; private set; }
    public string? TrabajoRealizado { get; private set; }
    public decimal? Costo { get; private set; }
    public string? NumeroFactura { get; private set; }
    public EstadoMantenimiento EstadoMantenimiento { get; private set; }

    protected Mantenimiento() { }

    private Mantenimiento(
        int idActivo,
        int idTipoMantenimiento,
        int? idProveedor,
        DateTime fechaProgramada,
        DateTime? fechaRealizado,
        string? responsable,
        string? descripcionProblema,
        string? trabajoRealizado,
        decimal? costo,
        string? numeroFactura,
        EstadoMantenimiento estadoMantenimiento)
    {
        IdActivo = idActivo;
        IdTipoMantenimiento = idTipoMantenimiento;
        IdProveedor = idProveedor;
        FechaProgramada = fechaProgramada;
        FechaRealizado = fechaRealizado;
        Responsable = responsable;
        DescripcionProblema = descripcionProblema;
        TrabajoRealizado = trabajoRealizado;
        Costo = costo;
        NumeroFactura = numeroFactura;
        EstadoMantenimiento = estadoMantenimiento;
    }

    public static Mantenimiento Crear(
        int idActivo,
        int idTipoMantenimiento,
        DateTime fechaProgramada,
        int? idProveedor = null,
        DateTime? fechaRealizado = null,
        string? responsable = null,
        string? descripcionProblema = null,
        string? trabajoRealizado = null,
        decimal? costo = null,
        string? numeroFactura = null,
        EstadoMantenimiento estadoMantenimiento = EstadoMantenimiento.Programado)
    {
        ValidarIdActivo(idActivo);
        ValidarIdTipoMantenimiento(idTipoMantenimiento);
        ValidarIdProveedor(idProveedor);
        ValidarFechaProgramada(fechaProgramada);
        ValidarFechaRealizado(fechaRealizado);
        ValidarCosto(costo);
        ValidarConsistenciaEstadoYFechas(estadoMantenimiento, fechaRealizado);

        return new Mantenimiento(
            idActivo,
            idTipoMantenimiento,
            idProveedor,
            fechaProgramada.Date,
            fechaRealizado?.Date,
            NormalizarResponsable(responsable),
            NormalizarDescripcionProblema(descripcionProblema),
            NormalizarTrabajoRealizado(trabajoRealizado),
            costo,
            NormalizarNumeroFactura(numeroFactura),
            estadoMantenimiento);
    }

    public void ActualizarDatos(
        int idTipoMantenimiento,
        DateTime fechaProgramada,
        int? idProveedor,
        DateTime? fechaRealizado,
        string? responsable,
        string? descripcionProblema,
        string? trabajoRealizado,
        decimal? costo,
        string? numeroFactura,
        EstadoMantenimiento estadoMantenimiento)
    {
        AsegurarQueNoEstaCancelado();
        ValidarIdTipoMantenimiento(idTipoMantenimiento);
        ValidarIdProveedor(idProveedor);
        ValidarFechaProgramada(fechaProgramada);
        ValidarFechaRealizado(fechaRealizado);
        ValidarCosto(costo);
        ValidarConsistenciaEstadoYFechas(estadoMantenimiento, fechaRealizado);

        IdTipoMantenimiento = idTipoMantenimiento;
        IdProveedor = idProveedor;
        FechaProgramada = fechaProgramada.Date;
        FechaRealizado = fechaRealizado?.Date;
        Responsable = NormalizarResponsable(responsable);
        DescripcionProblema = NormalizarDescripcionProblema(descripcionProblema);
        TrabajoRealizado = NormalizarTrabajoRealizado(trabajoRealizado);
        Costo = costo;
        NumeroFactura = NormalizarNumeroFactura(numeroFactura);
        EstadoMantenimiento = estadoMantenimiento;
    }

    /// <summary>
    /// Marca el mantenimiento como realizado. La fecha de realización, si
    /// no se informa, se toma como la fecha UTC de hoy.
    /// </summary>
    public void Completar(
        DateTime? fechaRealizado = null,
        string? trabajoRealizado = null,
        decimal? costo = null,
        string? numeroFactura = null,
        int? idProveedor = null)
    {
        AsegurarQueNoEstaCancelado();
        if (EstadoMantenimiento == EstadoMantenimiento.Completado)
            throw new InvalidOperationException("El mantenimiento ya está completado.");

        var fecha = (fechaRealizado ?? DateTime.UtcNow).Date;
        ValidarFechaRealizado(fecha);
        ValidarCosto(costo ?? Costo);
        ValidarIdProveedor(idProveedor ?? IdProveedor);

        FechaRealizado = fecha;
        TrabajoRealizado = NormalizarTrabajoRealizado(trabajoRealizado) ?? TrabajoRealizado;
        if (costo is not null)
            Costo = costo;
        if (numeroFactura is not null)
            NumeroFactura = NormalizarNumeroFactura(numeroFactura);
        if (idProveedor is not null)
            IdProveedor = idProveedor;

        EstadoMantenimiento = EstadoMantenimiento.Completado;
    }

    public void Cancelar()
    {
        if (EstadoMantenimiento == EstadoMantenimiento.Completado)
            throw new InvalidOperationException("No se puede cancelar un mantenimiento ya completado.");
        if (EstadoMantenimiento == EstadoMantenimiento.Cancelado)
            throw new InvalidOperationException("El mantenimiento ya está cancelado.");

        EstadoMantenimiento = EstadoMantenimiento.Cancelado;
        FechaRealizado = null;
    }

    private void AsegurarQueNoEstaCancelado()
    {
        if (EstadoMantenimiento == EstadoMantenimiento.Cancelado)
            throw new InvalidOperationException("No se puede modificar un mantenimiento cancelado.");
    }

    private static void ValidarIdActivo(int idActivo)
    {
        if (idActivo <= 0)
            throw new ArgumentException("El activo es obligatorio.", nameof(idActivo));
    }

    private static void ValidarIdTipoMantenimiento(int idTipoMantenimiento)
    {
        if (idTipoMantenimiento <= 0)
            throw new ArgumentException("El tipo de mantenimiento es obligatorio.", nameof(idTipoMantenimiento));
    }

    private static void ValidarIdProveedor(int? idProveedor)
    {
        if (idProveedor is <= 0)
            throw new ArgumentException("El proveedor, si se informa, debe ser mayor a 0.", nameof(idProveedor));
    }

    private static void ValidarFechaProgramada(DateTime fechaProgramada)
    {
        if (fechaProgramada == default)
            throw new ArgumentException("La fecha programada es obligatoria.", nameof(fechaProgramada));
    }

    private static void ValidarFechaRealizado(DateTime? fechaRealizado)
    {
        if (fechaRealizado is null)
            return;
        if (fechaRealizado == default)
            throw new ArgumentException("La fecha de realización no es válida.", nameof(fechaRealizado));
        if (fechaRealizado.Value.Date > DateTime.UtcNow.Date)
            throw new ArgumentException("La fecha de realización no puede ser futura.", nameof(fechaRealizado));
    }

    private static void ValidarCosto(decimal? costo)
    {
        if (costo is null)
            return;
        if (costo < 0)
            throw new ArgumentException("El costo no puede ser negativo.", nameof(costo));
        if (costo > CostoMaximo)
            throw new ArgumentException("El costo no puede exceder 9,999,999,999.99.", nameof(costo));
        if (decimal.Round(costo.Value, 2) != costo.Value)
            throw new ArgumentException("El costo no puede tener más de 2 decimales.", nameof(costo));
    }

    private static void ValidarConsistenciaEstadoYFechas(
        EstadoMantenimiento estadoMantenimiento,
        DateTime? fechaRealizado)
    {
        if (!Enum.IsDefined(estadoMantenimiento))
            throw new ArgumentException("El estado del mantenimiento no es válido.", nameof(estadoMantenimiento));

        if (estadoMantenimiento == EstadoMantenimiento.Completado && fechaRealizado is null)
            throw new ArgumentException("Un mantenimiento completado debe informar la fecha de realización.", nameof(fechaRealizado));
    }

    private static string? NormalizarResponsable(string? responsable)
    {
        if (string.IsNullOrWhiteSpace(responsable))
            return null;

        responsable = responsable.Trim();
        if (responsable.Length > 150)
            throw new ArgumentException("El responsable no puede exceder 150 caracteres.", nameof(responsable));

        return responsable;
    }

    private static string? NormalizarDescripcionProblema(string? descripcionProblema)
    {
        if (string.IsNullOrWhiteSpace(descripcionProblema))
            return null;

        descripcionProblema = descripcionProblema.Trim();
        if (descripcionProblema.Length > 300)
            throw new ArgumentException("La descripción del problema no puede exceder 300 caracteres.", nameof(descripcionProblema));

        return descripcionProblema;
    }

    private static string? NormalizarTrabajoRealizado(string? trabajoRealizado)
    {
        if (string.IsNullOrWhiteSpace(trabajoRealizado))
            return null;

        trabajoRealizado = trabajoRealizado.Trim();
        if (trabajoRealizado.Length > 150)
            throw new ArgumentException("El trabajo realizado no puede exceder 150 caracteres.", nameof(trabajoRealizado));

        return trabajoRealizado;
    }

    private static string? NormalizarNumeroFactura(string? numeroFactura)
    {
        if (string.IsNullOrWhiteSpace(numeroFactura))
            return null;

        numeroFactura = numeroFactura.Trim();
        if (numeroFactura.Length > 50)
            throw new ArgumentException("El número de factura no puede exceder 50 caracteres.", nameof(numeroFactura));

        return numeroFactura;
    }
}
