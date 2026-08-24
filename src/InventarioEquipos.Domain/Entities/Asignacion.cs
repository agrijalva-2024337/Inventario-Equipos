using InventarioEquipos.Domain.Common;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Asignación de un Activo a un Responsable, en una Ubicación de uso.
/// Depende de Activo, Responsable y Ubicacion, por lo que solo puede
/// mergearse después de que las tres estén en main. Columnas según el
/// diagrama: id_activo, id_responsable, id_ubicacion_uso, fecha_asignacion,
/// entregado_por, recibido_por, fecha_devolucion, activa, observaciones.
///
/// Las FKs se nombran Id{Entidad} (IdActivo, IdResponsable,
/// IdUbicacionUso) para que el orden de palabras coincida con el del
/// diagrama al pasar a snake_case en el DbContext (mismo criterio que
/// Sede, Ubicacion, Activo, etc.).
///
/// Regla de negocio del DERCAS que NO se valida aquí porque cruza varias
/// filas ("un activo solamente podrá tener una asignación activa"): se
/// garantiza con un índice único filtrado en Persistence
/// (IdActivo único WHERE activa = 1), y/o revisando en el servicio de
/// aplicación que no exista ya una asignación activa para ese Activo antes
/// de llamar Crear(...).
/// </summary>
public class Asignacion : EntityBase
{
    public int IdActivo { get; private set; }
    public Activo? Activo { get; private set; }

    /// <summary>Persona a la que se le asigna el activo.</summary>
    public int IdResponsable { get; private set; }
    public Responsable? Responsable { get; private set; }

    /// <summary>Ubicación donde se va a usar el activo (puede diferir de la ubicación "de resguardo").</summary>
    public int IdUbicacionUso { get; private set; }
    public Ubicacion? UbicacionUso { get; private set; }

    public DateTime FechaAsignacion { get; private set; }
    public string EntregadoPor { get; private set; } = default!;
    public string RecibidoPor { get; private set; } = default!;

    public DateTime? FechaDevolucion { get; private set; }

    /// <summary>true mientras el activo sigue en manos de este responsable; false una vez devuelto.</summary>
    public bool Activa { get; private set; }

    public string? Observaciones { get; private set; }

    protected Asignacion() { }

    private Asignacion(
        int idActivo,
        int idResponsable,
        int idUbicacionUso,
        string entregadoPor,
        string recibidoPor,
        string? observaciones)
    {
        IdActivo = idActivo;
        IdResponsable = idResponsable;
        IdUbicacionUso = idUbicacionUso;
        FechaAsignacion = DateTime.UtcNow;
        EntregadoPor = entregadoPor;
        RecibidoPor = recibidoPor;
        Activa = true;
        Observaciones = observaciones;
    }

    public static Asignacion Crear(
        int idActivo,
        int idResponsable,
        int idUbicacionUso,
        string entregadoPor,
        string recibidoPor,
        string? observaciones = null)
    {
        ValidarIdActivo(idActivo);
        ValidarIdResponsable(idResponsable);
        ValidarIdUbicacionUso(idUbicacionUso);
        ValidarEntregadoPor(entregadoPor);
        ValidarRecibidoPor(recibidoPor);
        ValidarObservaciones(observaciones);

        return new Asignacion(
            idActivo,
            idResponsable,
            idUbicacionUso,
            entregadoPor.Trim(),
            recibidoPor.Trim(),
            observaciones?.Trim());
    }

    /// <summary>
    /// Marca la asignación como devuelta. Una vez devuelta, deja de contar
    /// como "asignación activa" del activo (Activo.IdResponsable debería
    /// limpiarse aparte, desde el servicio de aplicación que orquesta la
    /// devolución, ya que Activo es una entidad/aggregate distinta).
    /// </summary>
    public void Devolver(DateTime? fechaDevolucion = null)
    {
        if (!Activa)
            throw new InvalidOperationException("Esta asignación ya fue devuelta anteriormente.");

        FechaDevolucion = fechaDevolucion ?? DateTime.UtcNow;
        Activa = false;
    }

    public void ActualizarObservaciones(string? observaciones)
    {
        ValidarObservaciones(observaciones);
        Observaciones = observaciones?.Trim();
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

    private static void ValidarEntregadoPor(string entregadoPor)
    {
        if (string.IsNullOrWhiteSpace(entregadoPor))
            throw new ArgumentException("Quién entrega el activo es obligatorio.", nameof(entregadoPor));
        if (entregadoPor.Length > 150)
            throw new ArgumentException("\"Entregado por\" no puede exceder 150 caracteres.", nameof(entregadoPor));
    }

    private static void ValidarRecibidoPor(string recibidoPor)
    {
        if (string.IsNullOrWhiteSpace(recibidoPor))
            throw new ArgumentException("Quién recibe el activo es obligatorio.", nameof(recibidoPor));
        if (recibidoPor.Length > 150)
            throw new ArgumentException("\"Recibido por\" no puede exceder 150 caracteres.", nameof(recibidoPor));
    }

    private static void ValidarObservaciones(string? observaciones)
    {
        if (observaciones is not null && observaciones.Trim().Length > 300)
            throw new ArgumentException("Las observaciones no pueden exceder 300 caracteres.", nameof(observaciones));
    }
}
