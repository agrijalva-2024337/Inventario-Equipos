using InventarioEquipos.Domain.Common;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Movimiento de un activo de una ubicación a otra. Depende de Activo y
/// de Ubicacion (origen y destino). Columnas según el diagrama:
/// id_activo, id_ubicacion_origen, id_ubicacion_destino, fecha_traslado,
/// motivo, responsable_traslado. El PK del diagrama es id_traslado.
/// Las FKs se nombran IdActivo / IdUbicacionOrigen / IdUbicacionDestino
/// para que el orden de palabras coincida con id_activo /
/// id_ubicacion_origen / id_ubicacion_destino al pasar a snake_case en
/// el DbContext.
///
/// Es de solo-registro (append-only): una vez creado no se actualiza ni
/// se borra, por eso no expone métodos de edición. El diagrama no incluye
/// columna estado, por eso no usa Activar/Desactivar.
///
/// Reglas de negocio del DERCAS que NO se validan aquí porque cruzan
/// varias filas / varias entidades (van en Application):
/// - "Un activo dado de baja no podrá asignarse, trasladarse ni enviarse
///   a mantenimiento": requiere conocer el catálogo EstadoActivo vigente.
/// - El origen debe coincidir con la ubicación actual del activo.
/// - Destino (y origen) deben existir y estar activos en Ubicacion.
/// - Actualizar la ubicación actual del activo (Sede/Ubicacion/Area) se
///   hace con Activo.ActualizarUbicacionActual después de registrar el
///   traslado, no dentro de esta entidad aislada.
/// </summary>
public class Traslado : EntityBase
{
    public int IdActivo { get; private set; }
    public Activo? Activo { get; private set; }

    public int IdUbicacionOrigen { get; private set; }
    public Ubicacion? UbicacionOrigen { get; private set; }

    public int IdUbicacionDestino { get; private set; }
    public Ubicacion? UbicacionDestino { get; private set; }

    public DateTime FechaTraslado { get; private set; }
    public string? Motivo { get; private set; }
    public string? ResponsableTraslado { get; private set; }

    protected Traslado() { }

    private Traslado(
        int idActivo,
        int idUbicacionOrigen,
        int idUbicacionDestino,
        DateTime fechaTraslado,
        string? motivo,
        string? responsableTraslado)
    {
        IdActivo = idActivo;
        IdUbicacionOrigen = idUbicacionOrigen;
        IdUbicacionDestino = idUbicacionDestino;
        FechaTraslado = fechaTraslado;
        Motivo = motivo;
        ResponsableTraslado = responsableTraslado;
    }

    public static Traslado Crear(
        int idActivo,
        int idUbicacionOrigen,
        int idUbicacionDestino,
        string? motivo = null,
        string? responsableTraslado = null,
        DateTime? fechaTraslado = null)
    {
        ValidarIdActivo(idActivo);
        ValidarIdUbicacionOrigen(idUbicacionOrigen);
        ValidarIdUbicacionDestino(idUbicacionDestino);
        ValidarUbicacionesDistintas(idUbicacionOrigen, idUbicacionDestino);

        return new Traslado(
            idActivo,
            idUbicacionOrigen,
            idUbicacionDestino,
            ResolverFechaTraslado(fechaTraslado),
            NormalizarMotivo(motivo),
            NormalizarResponsableTraslado(responsableTraslado));
    }

    private static void ValidarIdActivo(int idActivo)
    {
        if (idActivo <= 0)
            throw new ArgumentException("El activo es obligatorio.", nameof(idActivo));
    }

    private static void ValidarIdUbicacionOrigen(int idUbicacionOrigen)
    {
        if (idUbicacionOrigen <= 0)
            throw new ArgumentException("La ubicación de origen es obligatoria.", nameof(idUbicacionOrigen));
    }

    private static void ValidarIdUbicacionDestino(int idUbicacionDestino)
    {
        if (idUbicacionDestino <= 0)
            throw new ArgumentException("La ubicación de destino es obligatoria.", nameof(idUbicacionDestino));
    }

    private static void ValidarUbicacionesDistintas(int idUbicacionOrigen, int idUbicacionDestino)
    {
        if (idUbicacionOrigen == idUbicacionDestino)
            throw new ArgumentException("La ubicación de destino debe ser distinta a la de origen.", nameof(idUbicacionDestino));
    }

    private static DateTime ResolverFechaTraslado(DateTime? fechaTraslado)
    {
        var fecha = fechaTraslado ?? DateTime.UtcNow;
        if (fecha == default)
            throw new ArgumentException("La fecha del traslado no es válida.", nameof(fechaTraslado));
        if (fecha > DateTime.UtcNow.AddMinutes(1))
            throw new ArgumentException("La fecha del traslado no puede ser futura.", nameof(fechaTraslado));

        return fecha;
    }

    private static string? NormalizarMotivo(string? motivo)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            return null;

        motivo = motivo.Trim();
        if (motivo.Length > 200)
            throw new ArgumentException("El motivo no puede exceder 200 caracteres.", nameof(motivo));

        return motivo;
    }

    private static string? NormalizarResponsableTraslado(string? responsableTraslado)
    {
        if (string.IsNullOrWhiteSpace(responsableTraslado))
            return null;

        responsableTraslado = responsableTraslado.Trim();
        if (responsableTraslado.Length > 150)
            throw new ArgumentException("El responsable del traslado no puede exceder 150 caracteres.", nameof(responsableTraslado));

        return responsableTraslado;
    }
}