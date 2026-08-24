using InventarioEquipos.Domain.Common;
using InventarioEquipos.Domain.Enums;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Bitácora de auditoría de negocio (quién cambió qué, cuándo y con qué
/// datos antes/después). Depende de Usuario y Empresa. Es de solo-registro
/// (append-only): una vez creada no se actualiza ni se borra, por eso no
/// expone métodos de edición como las demás entidades. Columnas según el
/// diagrama: id_usuario, id_empresa, fecha_hora, tipo_operacion,
/// entidad_afectada, id_registro_afectado, informacion_anterior,
/// informacion_nueva. Las FKs se nombran IdUsuario / IdEmpresa para que
/// coincidan con id_usuario / id_empresa al pasar a snake_case.
/// </summary>
public class HistorialCambio : EntityBase
{
    public int IdUsuario { get; private set; }
    public Usuario? Usuario { get; private set; }

    public int IdEmpresa { get; private set; }
    public Empresa? Empresa { get; private set; }

    public DateTime FechaHora { get; private set; }

    public TipoOperacionHistorial TipoOperacion { get; private set; }

    /// <summary>Nombre de la entidad/tabla afectada, ej. "Empresa", "Usuario".</summary>
    public string EntidadAfectada { get; private set; } = default!;

    /// <summary>Id (INT) del registro afectado dentro de EntidadAfectada.</summary>
    public int IdRegistroAfectado { get; private set; }

    /// <summary>Snapshot (por ejemplo JSON serializado) del estado anterior. Null en creaciones.</summary>
    public string? InformacionAnterior { get; private set; }

    /// <summary>Snapshot del estado nuevo. Null en eliminaciones.</summary>
    public string? InformacionNueva { get; private set; }

    protected HistorialCambio() { }

    private HistorialCambio(
        int idUsuario,
        int idEmpresa,
        TipoOperacionHistorial tipoOperacion,
        string entidadAfectada,
        int idRegistroAfectado,
        string? informacionAnterior,
        string? informacionNueva)
    {
        IdUsuario = idUsuario;
        IdEmpresa = idEmpresa;
        FechaHora = DateTime.UtcNow;
        TipoOperacion = tipoOperacion;
        EntidadAfectada = entidadAfectada;
        IdRegistroAfectado = idRegistroAfectado;
        InformacionAnterior = informacionAnterior;
        InformacionNueva = informacionNueva;
    }

    public static HistorialCambio Registrar(
        int idUsuario,
        int idEmpresa,
        TipoOperacionHistorial tipoOperacion,
        string entidadAfectada,
        int idRegistroAfectado,
        string? informacionAnterior,
        string? informacionNueva)
    {
        if (idUsuario <= 0)
            throw new ArgumentException("El usuario que ejecuta la operación es obligatorio.", nameof(idUsuario));
        if (idEmpresa <= 0)
            throw new ArgumentException("La empresa es obligatoria.", nameof(idEmpresa));
        if (string.IsNullOrWhiteSpace(entidadAfectada))
            throw new ArgumentException("La entidad afectada es obligatoria.", nameof(entidadAfectada));
        if (entidadAfectada.Length > 100)
            throw new ArgumentException("La entidad afectada no puede exceder 100 caracteres.", nameof(entidadAfectada));
        if (idRegistroAfectado <= 0)
            throw new ArgumentException("El id del registro afectado es obligatorio.", nameof(idRegistroAfectado));

        if (tipoOperacion == TipoOperacionHistorial.Creacion && informacionNueva is null)
            throw new ArgumentException("Una creación debe incluir la información nueva.", nameof(informacionNueva));
        if (tipoOperacion == TipoOperacionHistorial.Eliminacion && informacionAnterior is null)
            throw new ArgumentException("Una eliminación debe incluir la información anterior.", nameof(informacionAnterior));

        return new HistorialCambio(
            idUsuario,
            idEmpresa,
            tipoOperacion,
            entidadAfectada.Trim(),
            idRegistroAfectado,
            informacionAnterior,
            informacionNueva);
    }
}
