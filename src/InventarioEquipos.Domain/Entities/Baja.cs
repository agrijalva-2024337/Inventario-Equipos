using InventarioEquipos.Domain.Common;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Registro de baja de un activo (venta, robo, daño, fin de vida útil,
/// etc.). Depende de Activo y MotivoBaja. Columnas según el diagrama:
/// id_activo, id_motivo_baja, fecha_baja, documento_referencia,
/// autorizado_por, observaciones. El PK del diagrama es id_baja.
/// Las FKs se nombran IdActivo / IdMotivoBaja para que el orden de
/// palabras coincida con id_activo / id_motivo_baja al pasar a
/// snake_case en el DbContext.
///
/// Es de solo-registro (append-only): una vez creada no se actualiza ni
/// se borra, por eso no expone métodos de edición. El diagrama no incluye
/// columna estado, por eso no usa Activar/Desactivar.
///
/// Reglas de negocio del DERCAS que NO se validan aquí porque cruzan
/// varias filas / varias entidades (van en Application):
/// - Un activo ya dado de baja no puede darse de baja otra vez: requiere
///   conocer el catálogo EstadoActivo vigente.
/// - El motivo debe existir, estar vigente y pertenecer a la misma
///   empresa del activo.
/// - Cambiar el estado del activo a "De baja" se hace con
///   Activo.CambiarEstado después de registrar la baja, no dentro de
///   esta entidad aislada.
/// </summary>
public class Baja : EntityBase
{
    public int IdActivo { get; private set; }
    public Activo? Activo { get; private set; }

    public int IdMotivoBaja { get; private set; }
    public MotivoBaja? MotivoBaja { get; private set; }

    public DateTime FechaBaja { get; private set; }
    public string? DocumentoReferencia { get; private set; }
    public string? AutorizadoPor { get; private set; }
    public string? Observaciones { get; private set; }

    protected Baja() { }

    private Baja(
        int idActivo,
        int idMotivoBaja,
        DateTime fechaBaja,
        string? documentoReferencia,
        string? autorizadoPor,
        string? observaciones)
    {
        IdActivo = idActivo;
        IdMotivoBaja = idMotivoBaja;
        FechaBaja = fechaBaja;
        DocumentoReferencia = documentoReferencia;
        AutorizadoPor = autorizadoPor;
        Observaciones = observaciones;
    }

    public static Baja Crear(
        int idActivo,
        int idMotivoBaja,
        string? documentoReferencia = null,
        string? autorizadoPor = null,
        string? observaciones = null,
        DateTime? fechaBaja = null)
    {
        ValidarIdActivo(idActivo);
        ValidarIdMotivoBaja(idMotivoBaja);

        return new Baja(
            idActivo,
            idMotivoBaja,
            ResolverFechaBaja(fechaBaja),
            NormalizarDocumentoReferencia(documentoReferencia),
            NormalizarAutorizadoPor(autorizadoPor),
            NormalizarObservaciones(observaciones));
    }

    private static void ValidarIdActivo(int idActivo)
    {
        if (idActivo <= 0)
            throw new ArgumentException("El activo es obligatorio.", nameof(idActivo));
    }

    private static void ValidarIdMotivoBaja(int idMotivoBaja)
    {
        if (idMotivoBaja <= 0)
            throw new ArgumentException("El motivo de baja es obligatorio.", nameof(idMotivoBaja));
    }

    private static DateTime ResolverFechaBaja(DateTime? fechaBaja)
    {
        var fecha = (fechaBaja ?? DateTime.UtcNow).Date;
        if (fecha == default)
            throw new ArgumentException("La fecha de baja no es válida.", nameof(fechaBaja));
        if (fecha > DateTime.UtcNow.Date)
            throw new ArgumentException("La fecha de baja no puede ser futura.", nameof(fechaBaja));

        return fecha;
    }

    private static string? NormalizarDocumentoReferencia(string? documentoReferencia)
    {
        if (string.IsNullOrWhiteSpace(documentoReferencia))
            return null;

        documentoReferencia = documentoReferencia.Trim();
        if (documentoReferencia.Length > 100)
            throw new ArgumentException("El documento de referencia no puede exceder 100 caracteres.", nameof(documentoReferencia));

        return documentoReferencia;
    }

    private static string? NormalizarAutorizadoPor(string? autorizadoPor)
    {
        if (string.IsNullOrWhiteSpace(autorizadoPor))
            return null;

        autorizadoPor = autorizadoPor.Trim();
        if (autorizadoPor.Length > 150)
            throw new ArgumentException("Quién autoriza la baja no puede exceder 150 caracteres.", nameof(autorizadoPor));

        return autorizadoPor;
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
