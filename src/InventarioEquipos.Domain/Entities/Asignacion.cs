using InventarioEquipos.Domain.Common;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Asignación de un activo a un responsable y una ubicación de uso.
/// Depende de Activo, Responsable y Ubicacion. Columnas según el diagrama:
/// id_activo, id_responsable, id_ubicacion_uso, fecha_asignacion,
/// entregado_por, recibido_por, fecha_devolucion, activa, observaciones.
/// El PK del diagrama es id_asignacion. Las FKs se nombran IdActivo /
/// IdResponsable / IdUbicacionUso para que coincidan con id_activo /
/// id_responsable / id_ubicacion_uso al pasar a snake_case en el DbContext.
///
/// El diagrama no incluye columna estado (Activo/Inactivo); el ciclo de
/// vida lo expresa el bit Activa (1 = vigente, 0 = devuelta).
///
/// Reglas de negocio del DERCAS que NO se validan aquí porque cruzan
/// varias filas / varias entidades (van en Application):
/// - "Un activo dado de baja no podrá asignarse, trasladarse ni enviarse
///   a mantenimiento": requiere conocer el catálogo EstadoActivo vigente.
/// - No puede haber dos asignaciones activas del mismo activo a la vez.
/// - Actualizar el responsable actual del activo se hace con
///   Activo.ActualizarResponsableActual después de registrar o devolver
///   la asignación, no dentro de esta entidad aislada.
/// </summary>
public class Asignacion : EntityBase
{
    public int IdActivo { get; private set; }
    public Activo? Activo { get; private set; }

    public int IdResponsable { get; private set; }
    public Responsable? Responsable { get; private set; }

    public int IdUbicacionUso { get; private set; }
    public Ubicacion? UbicacionUso { get; private set; }

    public DateTime FechaAsignacion { get; private set; }
    public string EntregadoPor { get; private set; } = default!;
    public string RecibidoPor { get; private set; } = default!;
    public DateTime? FechaDevolucion { get; private set; }
    public bool Activa { get; private set; }
    public string? Observaciones { get; private set; }

    protected Asignacion() { }

    private Asignacion(
        int idActivo,
        int idResponsable,
        int idUbicacionUso,
        DateTime fechaAsignacion,
        string entregadoPor,
        string recibidoPor,
        DateTime? fechaDevolucion,
        bool activa,
        string? observaciones)
    {
        IdActivo = idActivo;
        IdResponsable = idResponsable;
        IdUbicacionUso = idUbicacionUso;
        FechaAsignacion = fechaAsignacion;
        EntregadoPor = entregadoPor;
        RecibidoPor = recibidoPor;
        FechaDevolucion = fechaDevolucion;
        Activa = activa;
        Observaciones = observaciones;
    }

    public static Asignacion Crear(
        int idActivo,
        int idResponsable,
        int idUbicacionUso,
        string entregadoPor,
        string recibidoPor,
        string? observaciones = null,
        DateTime? fechaAsignacion = null)
    {
        ValidarIdActivo(idActivo);
        ValidarIdResponsable(idResponsable);
        ValidarIdUbicacionUso(idUbicacionUso);

        return new Asignacion(
            idActivo,
            idResponsable,
            idUbicacionUso,
            ResolverFechaAsignacion(fechaAsignacion),
            ValidarNombrePersona(entregadoPor, nameof(entregadoPor), "Quién entrega"),
            ValidarNombrePersona(recibidoPor, nameof(recibidoPor), "Quién recibe"),
            fechaDevolucion: null,
            activa: true,
            NormalizarObservaciones(observaciones));
    }

    /// <summary>
    /// Marca la asignación como devuelta. La fecha de devolución, si no se
    /// informa, se toma como la fecha/hora UTC actual.
    /// </summary>
    public void Devolver(DateTime? fechaDevolucion = null, string? observaciones = null)
    {
        if (!Activa)
            throw new InvalidOperationException("La asignación ya está devuelta.");

        var fecha = ResolverFechaDevolucion(fechaDevolucion, FechaAsignacion);
        FechaDevolucion = fecha;
        Activa = false;
        if (observaciones is not null)
            Observaciones = NormalizarObservaciones(observaciones);
    }

    private static void ValidarIdActivo(int idActivo)
    {
        if (idActivo <= 0)
            throw new ArgumentException("El activo es obligatorio.", nameof(idActivo));
    }

    private static void ValidarIdResponsable(int idResponsable)
    {
        if (idResponsable <= 0)
            throw new ArgumentException("El responsable es obligatorio.", nameof(idResponsable));
    }

    private static void ValidarIdUbicacionUso(int idUbicacionUso)
    {
        if (idUbicacionUso <= 0)
            throw new ArgumentException("La ubicación de uso es obligatoria.", nameof(idUbicacionUso));
    }

    private static DateTime ResolverFechaAsignacion(DateTime? fechaAsignacion)
    {
        var fecha = fechaAsignacion ?? DateTime.UtcNow;
        if (fecha == default)
            throw new ArgumentException("La fecha de asignación no es válida.", nameof(fechaAsignacion));
        if (fecha > DateTime.UtcNow.AddMinutes(1))
            throw new ArgumentException("La fecha de asignación no puede ser futura.", nameof(fechaAsignacion));

        return fecha;
    }

    private static DateTime ResolverFechaDevolucion(DateTime? fechaDevolucion, DateTime fechaAsignacion)
    {
        var fecha = fechaDevolucion ?? DateTime.UtcNow;
        if (fecha == default)
            throw new ArgumentException("La fecha de devolución no es válida.", nameof(fechaDevolucion));
        if (fecha < fechaAsignacion)
            throw new ArgumentException("La fecha de devolución no puede ser anterior a la de asignación.", nameof(fechaDevolucion));
        if (fecha > DateTime.UtcNow.AddMinutes(1))
            throw new ArgumentException("La fecha de devolución no puede ser futura.", nameof(fechaDevolucion));

        return fecha;
    }

    private static string ValidarNombrePersona(string valor, string paramName, string etiqueta)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException($"{etiqueta} es obligatorio.", paramName);

        valor = valor.Trim();
        if (valor.Length > 150)
            throw new ArgumentException($"{etiqueta} no puede exceder 150 caracteres.", paramName);

        return valor;
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
