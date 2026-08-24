using InventarioEquipos.Domain.Common;
using InventarioEquipos.Domain.Enums;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Conteo físico de activos de una empresa, opcionalmente acotado a una
/// sede y/o ubicación. Depende de Empresa y Usuario (obligatorios) y de
/// Sede y Ubicacion (opcionales: un inventario puede cubrir toda la
/// empresa). Columnas según el diagrama: id_empresa, id_sede,
/// id_ubicacion, fecha_inicio, fecha_cierre, estado,
/// id_usuario_responsable, observaciones. El PK del diagrama es
/// id_inventario. Las FKs se nombran IdEmpresa / IdSede / IdUbicacion /
/// IdUsuarioResponsable para que el orden de palabras coincida con
/// id_empresa / id_sede / id_ubicacion / id_usuario_responsable al pasar
/// a snake_case en el DbContext.
///
/// El diagrama no incluye columna estado (Activo/Inactivo), por eso no
/// usa Activar/Desactivar; el ciclo de vida lo expresa
/// EstadoInventarioFisico (Abierto, Cerrado, Cancelado).
///
/// Reglas de negocio que NO se validan aquí porque cruzan varias filas /
/// varias entidades (van en Application):
/// - La empresa, la sede, la ubicación y el usuario deben existir y
///   estar vigentes.
/// - La sede (si se informa) debe pertenecer a la misma empresa.
/// - La ubicación (si se informa) debe pertenecer a la sede indicada.
/// - El usuario responsable debe estar asignado a esa empresa.
/// - No puede haber dos inventarios Abiertos con el mismo alcance
///   (empresa / sede / ubicación) al mismo tiempo.
/// </summary>
public class InventarioFisico : EntityBase
{
    public int IdEmpresa { get; private set; }
    public Empresa? Empresa { get; private set; }

    public int? IdSede { get; private set; }
    public Sede? Sede { get; private set; }

    public int? IdUbicacion { get; private set; }
    public Ubicacion? Ubicacion { get; private set; }

    public DateTime FechaInicio { get; private set; }
    public DateTime? FechaCierre { get; private set; }
    public EstadoInventarioFisico Estado { get; private set; }

    public int IdUsuarioResponsable { get; private set; }
    public Usuario? UsuarioResponsable { get; private set; }

    public string? Observaciones { get; private set; }

    protected InventarioFisico() { }

    private InventarioFisico(
        int idEmpresa,
        int? idSede,
        int? idUbicacion,
        DateTime fechaInicio,
        DateTime? fechaCierre,
        EstadoInventarioFisico estado,
        int idUsuarioResponsable,
        string? observaciones)
    {
        IdEmpresa = idEmpresa;
        IdSede = idSede;
        IdUbicacion = idUbicacion;
        FechaInicio = fechaInicio;
        FechaCierre = fechaCierre;
        Estado = estado;
        IdUsuarioResponsable = idUsuarioResponsable;
        Observaciones = observaciones;
    }

    public static InventarioFisico Crear(
        int idEmpresa,
        int idUsuarioResponsable,
        int? idSede = null,
        int? idUbicacion = null,
        string? observaciones = null,
        DateTime? fechaInicio = null)
    {
        ValidarIdEmpresa(idEmpresa);
        ValidarIdUsuarioResponsable(idUsuarioResponsable);
        ValidarIdSede(idSede);
        ValidarIdUbicacion(idUbicacion);
        ValidarAlcance(idSede, idUbicacion);

        return new InventarioFisico(
            idEmpresa,
            idSede,
            idUbicacion,
            ResolverFechaInicio(fechaInicio),
            fechaCierre: null,
            EstadoInventarioFisico.Abierto,
            idUsuarioResponsable,
            NormalizarObservaciones(observaciones));
    }

    public void ActualizarDatos(
        int idUsuarioResponsable,
        int? idSede,
        int? idUbicacion,
        string? observaciones)
    {
        AsegurarQueEstaAbierto();
        ValidarIdUsuarioResponsable(idUsuarioResponsable);
        ValidarIdSede(idSede);
        ValidarIdUbicacion(idUbicacion);
        ValidarAlcance(idSede, idUbicacion);

        IdUsuarioResponsable = idUsuarioResponsable;
        IdSede = idSede;
        IdUbicacion = idUbicacion;
        Observaciones = NormalizarObservaciones(observaciones);
    }

    /// <summary>
    /// Cierra el inventario. La fecha de cierre, si no se informa, se toma
    /// como la fecha/hora UTC actual.
    /// </summary>
    public void Cerrar(DateTime? fechaCierre = null, string? observaciones = null)
    {
        AsegurarQueEstaAbierto();

        var fecha = ResolverFechaCierre(fechaCierre, FechaInicio);
        FechaCierre = fecha;
        if (observaciones is not null)
            Observaciones = NormalizarObservaciones(observaciones);

        Estado = EstadoInventarioFisico.Cerrado;
    }

    public void Cancelar(string? observaciones = null)
    {
        AsegurarQueEstaAbierto();

        Estado = EstadoInventarioFisico.Cancelado;
        FechaCierre = null;
        if (observaciones is not null)
            Observaciones = NormalizarObservaciones(observaciones);
    }

    private void AsegurarQueEstaAbierto()
    {
        if (Estado == EstadoInventarioFisico.Cerrado)
            throw new InvalidOperationException("No se puede modificar un inventario físico ya cerrado.");
        if (Estado == EstadoInventarioFisico.Cancelado)
            throw new InvalidOperationException("No se puede modificar un inventario físico cancelado.");
    }

    private static void ValidarIdEmpresa(int idEmpresa)
    {
        if (idEmpresa <= 0)
            throw new ArgumentException("La empresa es obligatoria.", nameof(idEmpresa));
    }

    private static void ValidarIdUsuarioResponsable(int idUsuarioResponsable)
    {
        if (idUsuarioResponsable <= 0)
            throw new ArgumentException("El usuario responsable es obligatorio.", nameof(idUsuarioResponsable));
    }

    private static void ValidarIdSede(int? idSede)
    {
        if (idSede is <= 0)
            throw new ArgumentException("La sede, si se informa, debe ser mayor a 0.", nameof(idSede));
    }

    private static void ValidarIdUbicacion(int? idUbicacion)
    {
        if (idUbicacion is <= 0)
            throw new ArgumentException("La ubicación, si se informa, debe ser mayor a 0.", nameof(idUbicacion));
    }

    private static void ValidarAlcance(int? idSede, int? idUbicacion)
    {
        if (idUbicacion is not null && idSede is null)
            throw new ArgumentException("Si se informa una ubicación, la sede es obligatoria.", nameof(idSede));
    }

    private static DateTime ResolverFechaInicio(DateTime? fechaInicio)
    {
        var fecha = fechaInicio ?? DateTime.UtcNow;
        if (fecha == default)
            throw new ArgumentException("La fecha de inicio no es válida.", nameof(fechaInicio));
        if (fecha > DateTime.UtcNow.AddMinutes(1))
            throw new ArgumentException("La fecha de inicio no puede ser futura.", nameof(fechaInicio));

        return fecha;
    }

    private static DateTime ResolverFechaCierre(DateTime? fechaCierre, DateTime fechaInicio)
    {
        var fecha = fechaCierre ?? DateTime.UtcNow;
        if (fecha == default)
            throw new ArgumentException("La fecha de cierre no es válida.", nameof(fechaCierre));
        if (fecha < fechaInicio)
            throw new ArgumentException("La fecha de cierre no puede ser anterior a la de inicio.", nameof(fechaCierre));
        if (fecha > DateTime.UtcNow.AddMinutes(1))
            throw new ArgumentException("La fecha de cierre no puede ser futura.", nameof(fechaCierre));

        return fecha;
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
