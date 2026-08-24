using InventarioEquipos.Domain.Common;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Línea de un conteo físico: registra si un activo se encontró, dónde y
/// en qué estado. Depende de InventarioFisico y Activo (obligatorios) y
/// de Ubicacion (opcional: solo aplica si el activo se encontró).
/// Columnas según el diagrama: id_inventario, id_activo, encontrado,
/// id_ubicacion_encontrada, estado_fisico_observado, observaciones,
/// fecha_verificacion. El PK del diagrama es id_detalle. Las FKs se
/// nombran IdInventario / IdActivo / IdUbicacionEncontrada para que el
/// orden de palabras coincida con id_inventario / id_activo /
/// id_ubicacion_encontrada al pasar a snake_case en el DbContext.
///
/// El diagrama no incluye columna estado (Activo/Inactivo), por eso no
/// usa Activar/Desactivar. Una línea se puede actualizar mientras el
/// inventario padre siga abierto (eso se valida en Application).
///
/// Reglas de negocio que NO se validan aquí porque cruzan varias filas /
/// varias entidades (van en Application):
/// - El inventario, el activo y la ubicación (si se informa) deben
///   existir y estar vigentes.
/// - El inventario debe estar Abierto para crear o actualizar líneas.
/// - El activo debe pertenecer a la empresa (y al alcance sede /
///   ubicación, si el inventario está acotado).
/// - La ubicación encontrada debe pertenecer a la misma empresa.
/// - No puede haber dos líneas con el mismo activo dentro del mismo
///   inventario.
/// </summary>
public class DetalleInventario : EntityBase
{
    public int IdInventario { get; private set; }
    public InventarioFisico? InventarioFisico { get; private set; }

    public int IdActivo { get; private set; }
    public Activo? Activo { get; private set; }

    public bool Encontrado { get; private set; }

    public int? IdUbicacionEncontrada { get; private set; }
    public Ubicacion? UbicacionEncontrada { get; private set; }

    public string? EstadoFisicoObservado { get; private set; }
    public string? Observaciones { get; private set; }
    public DateTime FechaVerificacion { get; private set; }

    protected DetalleInventario() { }

    private DetalleInventario(
        int idInventario,
        int idActivo,
        bool encontrado,
        int? idUbicacionEncontrada,
        string? estadoFisicoObservado,
        string? observaciones,
        DateTime fechaVerificacion)
    {
        IdInventario = idInventario;
        IdActivo = idActivo;
        Encontrado = encontrado;
        IdUbicacionEncontrada = idUbicacionEncontrada;
        EstadoFisicoObservado = estadoFisicoObservado;
        Observaciones = observaciones;
        FechaVerificacion = fechaVerificacion;
    }

    public static DetalleInventario Crear(
        int idInventario,
        int idActivo,
        bool encontrado = false,
        int? idUbicacionEncontrada = null,
        string? estadoFisicoObservado = null,
        string? observaciones = null,
        DateTime? fechaVerificacion = null)
    {
        ValidarIdInventario(idInventario);
        ValidarIdActivo(idActivo);
        ValidarIdUbicacionEncontrada(idUbicacionEncontrada);
        ValidarConsistenciaHallazgo(encontrado, idUbicacionEncontrada, estadoFisicoObservado);

        return new DetalleInventario(
            idInventario,
            idActivo,
            encontrado,
            idUbicacionEncontrada,
            NormalizarEstadoFisicoObservado(estadoFisicoObservado),
            NormalizarObservaciones(observaciones),
            ResolverFechaVerificacion(fechaVerificacion));
    }

    /// <summary>
    /// Actualiza el resultado de la verificación. La fecha, si no se
    /// informa, se toma como la fecha/hora UTC actual.
    /// </summary>
    public void ActualizarVerificacion(
        bool encontrado,
        int? idUbicacionEncontrada,
        string? estadoFisicoObservado,
        string? observaciones,
        DateTime? fechaVerificacion = null)
    {
        ValidarIdUbicacionEncontrada(idUbicacionEncontrada);
        ValidarConsistenciaHallazgo(encontrado, idUbicacionEncontrada, estadoFisicoObservado);

        Encontrado = encontrado;
        IdUbicacionEncontrada = idUbicacionEncontrada;
        EstadoFisicoObservado = NormalizarEstadoFisicoObservado(estadoFisicoObservado);
        Observaciones = NormalizarObservaciones(observaciones);
        FechaVerificacion = ResolverFechaVerificacion(fechaVerificacion);
    }

    public void MarcarEncontrado(
        int idUbicacionEncontrada,
        string? estadoFisicoObservado = null,
        string? observaciones = null,
        DateTime? fechaVerificacion = null)
    {
        ActualizarVerificacion(
            encontrado: true,
            idUbicacionEncontrada,
            estadoFisicoObservado,
            observaciones,
            fechaVerificacion);
    }

    public void MarcarNoEncontrado(
        string? observaciones = null,
        DateTime? fechaVerificacion = null)
    {
        ActualizarVerificacion(
            encontrado: false,
            idUbicacionEncontrada: null,
            estadoFisicoObservado: null,
            observaciones,
            fechaVerificacion);
    }

    private static void ValidarIdInventario(int idInventario)
    {
        if (idInventario <= 0)
            throw new ArgumentException("El inventario es obligatorio.", nameof(idInventario));
    }

    private static void ValidarIdActivo(int idActivo)
    {
        if (idActivo <= 0)
            throw new ArgumentException("El activo es obligatorio.", nameof(idActivo));
    }

    private static void ValidarIdUbicacionEncontrada(int? idUbicacionEncontrada)
    {
        if (idUbicacionEncontrada is <= 0)
            throw new ArgumentException("La ubicación encontrada, si se informa, debe ser mayor a 0.", nameof(idUbicacionEncontrada));
    }

    private static void ValidarConsistenciaHallazgo(
        bool encontrado,
        int? idUbicacionEncontrada,
        string? estadoFisicoObservado)
    {
        if (encontrado)
        {
            if (idUbicacionEncontrada is null)
                throw new ArgumentException(
                    "Si el activo se encontró, la ubicación encontrada es obligatoria.",
                    nameof(idUbicacionEncontrada));
            return;
        }

        if (idUbicacionEncontrada is not null)
            throw new ArgumentException(
                "Si el activo no se encontró, no debe informarse una ubicación encontrada.",
                nameof(idUbicacionEncontrada));

        if (!string.IsNullOrWhiteSpace(estadoFisicoObservado))
            throw new ArgumentException(
                "Si el activo no se encontró, no debe informarse un estado físico observado.",
                nameof(estadoFisicoObservado));
    }

    private static DateTime ResolverFechaVerificacion(DateTime? fechaVerificacion)
    {
        var fecha = fechaVerificacion ?? DateTime.UtcNow;
        if (fecha == default)
            throw new ArgumentException("La fecha de verificación no es válida.", nameof(fechaVerificacion));
        if (fecha > DateTime.UtcNow.AddMinutes(1))
            throw new ArgumentException("La fecha de verificación no puede ser futura.", nameof(fechaVerificacion));

        return fecha;
    }

    private static string? NormalizarEstadoFisicoObservado(string? estadoFisicoObservado)
    {
        if (string.IsNullOrWhiteSpace(estadoFisicoObservado))
            return null;

        estadoFisicoObservado = estadoFisicoObservado.Trim();
        if (estadoFisicoObservado.Length > 100)
            throw new ArgumentException("El estado físico observado no puede exceder 100 caracteres.", nameof(estadoFisicoObservado));

        return estadoFisicoObservado;
    }

    private static string? NormalizarObservaciones(string? observaciones)
    {
        if (string.IsNullOrWhiteSpace(observaciones))
            return null;

        observaciones = observaciones.Trim();
        if (observaciones.Length > 300)
            throw new ArgumentException("Las observaciones no pueden exceder 300 caracteres.", nameof(observaciones));

        return observaciones;
    }
}
