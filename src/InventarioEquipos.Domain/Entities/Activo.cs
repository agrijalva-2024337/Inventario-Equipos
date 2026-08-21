using InventarioEquipos.Domain.Common;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Activo físico de una empresa: el corazón del sistema de inventario.
/// Depende de Empresa, CategoriaActivo, Sede, Ubicacion, Area y
/// EstadoActivo (todas obligatorias) y de Proveedor y Responsable
/// (opcionales: un activo puede registrarse sin proveedor conocido o sin
/// responsable asignado todavía). Columnas según el diagrama: id_empresa,
/// codigo_interno, nombre, descripcion, id_categoria, marca, modelo,
/// numero_serie, fecha_compra, costo_adquisicion, moneda, id_proveedor,
/// numero_factura, fecha_vencimiento_garantia, id_sede, id_ubicacion,
/// id_area, id_responsable, id_estado, observaciones.
///
/// Reglas de negocio del DERCAS que NO se validan aquí porque cruzan
/// varias filas / varias entidades (van en Application):
/// - "El código interno del activo deberá ser único dentro de la
///   empresa": se garantiza con un índice único compuesto
///   (EmpresaId, CodigoInterno) en la configuración de EF Core.
/// - "Un activo dado de baja no podrá asignarse, trasladarse ni enviarse
///   a mantenimiento": requiere conocer el catálogo EstadoActivo vigente,
///   se valida en el servicio de aplicación antes de crear una Asignacion,
///   Traslado o Mantenimiento.
/// </summary>
public class Activo : EntityBase
{
    public int EmpresaId { get; private set; }
    public Empresa? Empresa { get; private set; }

    public string CodigoInterno { get; private set; } = default!;
    public string Nombre { get; private set; } = default!;
    public string? Descripcion { get; private set; }

    public int CategoriaActivoId { get; private set; }
    public CategoriaActivo? CategoriaActivo { get; private set; }

    public string? Marca { get; private set; }
    public string? Modelo { get; private set; }
    public string? NumeroSerie { get; private set; }

    public DateTime? FechaCompra { get; private set; }
    public decimal? CostoAdquisicion { get; private set; }
    public string? Moneda { get; private set; }

    public int? ProveedorId { get; private set; }
    public Proveedor? Proveedor { get; private set; }

    public string? NumeroFactura { get; private set; }
    public DateTime? FechaVencimientoGarantia { get; private set; }

    public int SedeId { get; private set; }
    public Sede? Sede { get; private set; }

    /// <summary>Ubicación actual del activo (única en un momento dado).</summary>
    public int UbicacionId { get; private set; }
    public Ubicacion? Ubicacion { get; private set; }

    public int AreaId { get; private set; }
    public Area? Area { get; private set; }

    /// <summary>Responsable actual del activo; null si todavía no se ha asignado a nadie.</summary>
    public int? ResponsableId { get; private set; }
    public Responsable? Responsable { get; private set; }

    public int EstadoActivoId { get; private set; }
    public EstadoActivo? EstadoActivo { get; private set; }

    public string? Observaciones { get; private set; }

    protected Activo() { }

    private Activo(
        int empresaId,
        string codigoInterno,
        string nombre,
        string? descripcion,
        int categoriaActivoId,
        string? marca,
        string? modelo,
        string? numeroSerie,
        DateTime? fechaCompra,
        decimal? costoAdquisicion,
        string? moneda,
        int? proveedorId,
        string? numeroFactura,
        DateTime? fechaVencimientoGarantia,
        int sedeId,
        int ubicacionId,
        int areaId,
        int? responsableId,
        int estadoActivoId,
        string? observaciones)
    {
        EmpresaId = empresaId;
        CodigoInterno = codigoInterno;
        Nombre = nombre;
        Descripcion = descripcion;
        CategoriaActivoId = categoriaActivoId;
        Marca = marca;
        Modelo = modelo;
        NumeroSerie = numeroSerie;
        FechaCompra = fechaCompra;
        CostoAdquisicion = costoAdquisicion;
        Moneda = moneda;
        ProveedorId = proveedorId;
        NumeroFactura = numeroFactura;
        FechaVencimientoGarantia = fechaVencimientoGarantia;
        SedeId = sedeId;
        UbicacionId = ubicacionId;
        AreaId = areaId;
        ResponsableId = responsableId;
        EstadoActivoId = estadoActivoId;
        Observaciones = observaciones;
    }

    public static Activo Crear(
        int empresaId,
        string codigoInterno,
        string nombre,
        int categoriaActivoId,
        int sedeId,
        int ubicacionId,
        int areaId,
        int estadoActivoId,
        string? descripcion = null,
        string? marca = null,
        string? modelo = null,
        string? numeroSerie = null,
        DateTime? fechaCompra = null,
        decimal? costoAdquisicion = null,
        string? moneda = null,
        int? proveedorId = null,
        string? numeroFactura = null,
        DateTime? fechaVencimientoGarantia = null,
        int? responsableId = null,
        string? observaciones = null)
    {
        ValidarEmpresaId(empresaId);
        ValidarCodigoInterno(codigoInterno);
        ValidarNombre(nombre);
        ValidarCategoriaActivoId(categoriaActivoId);
        ValidarSedeId(sedeId);
        ValidarUbicacionId(ubicacionId);
        ValidarAreaId(areaId);
        ValidarEstadoActivoId(estadoActivoId);
        ValidarProveedorId(proveedorId);
        ValidarResponsableId(responsableId);
        ValidarCostoAdquisicion(costoAdquisicion);
        ValidarDescripcion(descripcion);
        ValidarMarca(marca);
        ValidarModelo(modelo);
        ValidarNumeroSerie(numeroSerie);
        ValidarMoneda(moneda);
        ValidarNumeroFactura(numeroFactura);
        ValidarObservaciones(observaciones);

        return new Activo(
            empresaId,
            codigoInterno.Trim(),
            nombre.Trim(),
            descripcion?.Trim(),
            categoriaActivoId,
            marca?.Trim(),
            modelo?.Trim(),
            numeroSerie?.Trim(),
            fechaCompra,
            costoAdquisicion,
            moneda?.Trim().ToUpperInvariant(),
            proveedorId,
            numeroFactura?.Trim(),
            fechaVencimientoGarantia,
            sedeId,
            ubicacionId,
            areaId,
            responsableId,
            estadoActivoId,
            observaciones?.Trim());
    }

    public void ActualizarDatos(
        string codigoInterno,
        string nombre,
        int categoriaActivoId,
        string? descripcion,
        string? marca,
        string? modelo,
        string? numeroSerie,
        DateTime? fechaCompra,
        decimal? costoAdquisicion,
        string? moneda,
        int? proveedorId,
        string? numeroFactura,
        DateTime? fechaVencimientoGarantia,
        string? observaciones)
    {
        ValidarCodigoInterno(codigoInterno);
        ValidarNombre(nombre);
        ValidarCategoriaActivoId(categoriaActivoId);
        ValidarProveedorId(proveedorId);
        ValidarCostoAdquisicion(costoAdquisicion);
        ValidarDescripcion(descripcion);
        ValidarMarca(marca);
        ValidarModelo(modelo);
        ValidarNumeroSerie(numeroSerie);
        ValidarMoneda(moneda);
        ValidarNumeroFactura(numeroFactura);
        ValidarObservaciones(observaciones);

        CodigoInterno = codigoInterno.Trim();
        Nombre = nombre.Trim();
        CategoriaActivoId = categoriaActivoId;
        Descripcion = descripcion?.Trim();
        Marca = marca?.Trim();
        Modelo = modelo?.Trim();
        NumeroSerie = numeroSerie?.Trim();
        FechaCompra = fechaCompra;
        CostoAdquisicion = costoAdquisicion;
        Moneda = moneda?.Trim().ToUpperInvariant();
        ProveedorId = proveedorId;
        NumeroFactura = numeroFactura?.Trim();
        FechaVencimientoGarantia = fechaVencimientoGarantia;
        Observaciones = observaciones?.Trim();
    }

    /// <summary>Usado por Traslados: cambia la ubicación (y opcionalmente sede/área) actual.</summary>
    public void ActualizarUbicacionActual(int sedeId, int ubicacionId, int areaId)
    {
        ValidarSedeId(sedeId);
        ValidarUbicacionId(ubicacionId);
        ValidarAreaId(areaId);

        SedeId = sedeId;
        UbicacionId = ubicacionId;
        AreaId = areaId;
    }

    /// <summary>Usado por Asignaciones: cambia quién es el responsable actual (null = sin asignar).</summary>
    public void ActualizarResponsableActual(int? responsableId)
    {
        ValidarResponsableId(responsableId);
        ResponsableId = responsableId;
    }

    /// <summary>
    /// Cambia el estado del activo dentro del catálogo EstadoActivo de su
    /// empresa (ej. "Disponible", "Asignado", "En mantenimiento", "De baja").
    /// No existe un booleano Activo/Inactivo separado: el estado del activo
    /// siempre es uno de los valores de ese catálogo.
    /// </summary>
    public void CambiarEstado(int estadoActivoId)
    {
        ValidarEstadoActivoId(estadoActivoId);
        EstadoActivoId = estadoActivoId;
    }

    private static void ValidarEmpresaId(int empresaId)
    {
        if (empresaId <= 0)
            throw new ArgumentException("La empresa es obligatoria.", nameof(empresaId));
    }

    private static void ValidarCodigoInterno(string codigoInterno)
    {
        if (string.IsNullOrWhiteSpace(codigoInterno))
            throw new ArgumentException("El código interno es obligatorio.", nameof(codigoInterno));
        if (codigoInterno.Length > 50)
            throw new ArgumentException("El código interno no puede exceder 50 caracteres.", nameof(codigoInterno));
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del activo es obligatorio.", nameof(nombre));
        if (nombre.Length > 150)
            throw new ArgumentException("El nombre del activo no puede exceder 150 caracteres.", nameof(nombre));
    }

    private static void ValidarCategoriaActivoId(int categoriaActivoId)
    {
        if (categoriaActivoId <= 0)
            throw new ArgumentException("La categoría es obligatoria.", nameof(categoriaActivoId));
    }

    private static void ValidarSedeId(int sedeId)
    {
        if (sedeId <= 0)
            throw new ArgumentException("La sede es obligatoria.", nameof(sedeId));
    }

    private static void ValidarUbicacionId(int ubicacionId)
    {
        if (ubicacionId <= 0)
            throw new ArgumentException("La ubicación es obligatoria.", nameof(ubicacionId));
    }

    private static void ValidarAreaId(int areaId)
    {
        if (areaId <= 0)
            throw new ArgumentException("El área es obligatoria.", nameof(areaId));
    }

    private static void ValidarEstadoActivoId(int estadoActivoId)
    {
        if (estadoActivoId <= 0)
            throw new ArgumentException("El estado del activo es obligatorio.", nameof(estadoActivoId));
    }

    private static void ValidarProveedorId(int? proveedorId)
    {
        if (proveedorId is <= 0)
            throw new ArgumentException("El proveedor, si se informa, debe ser mayor a 0.", nameof(proveedorId));
    }

    private static void ValidarResponsableId(int? responsableId)
    {
        if (responsableId is <= 0)
            throw new ArgumentException("El responsable, si se informa, debe ser mayor a 0.", nameof(responsableId));
    }

    private static void ValidarCostoAdquisicion(decimal? costoAdquisicion)
    {
        if (costoAdquisicion is < 0)
            throw new ArgumentException("El costo de adquisición no puede ser negativo.", nameof(costoAdquisicion));
    }

    private static void ValidarDescripcion(string? descripcion)
    {
        if (descripcion is not null && descripcion.Trim().Length > 300)
            throw new ArgumentException("La descripción no puede exceder 300 caracteres.", nameof(descripcion));
    }

    private static void ValidarMarca(string? marca)
    {
        if (marca is not null && marca.Trim().Length > 100)
            throw new ArgumentException("La marca no puede exceder 100 caracteres.", nameof(marca));
    }

    private static void ValidarModelo(string? modelo)
    {
        if (modelo is not null && modelo.Trim().Length > 100)
            throw new ArgumentException("El modelo no puede exceder 100 caracteres.", nameof(modelo));
    }

    private static void ValidarNumeroSerie(string? numeroSerie)
    {
        if (numeroSerie is not null && numeroSerie.Trim().Length > 100)
            throw new ArgumentException("El número de serie no puede exceder 100 caracteres.", nameof(numeroSerie));
    }

    private static void ValidarMoneda(string? moneda)
    {
        if (moneda is not null && moneda.Trim().Length > 10)
            throw new ArgumentException("La moneda no puede exceder 10 caracteres.", nameof(moneda));
    }

    private static void ValidarNumeroFactura(string? numeroFactura)
    {
        if (numeroFactura is not null && numeroFactura.Trim().Length > 50)
            throw new ArgumentException("El número de factura no puede exceder 50 caracteres.", nameof(numeroFactura));
    }

    private static void ValidarObservaciones(string? observaciones)
    {
        if (observaciones is not null && observaciones.Trim().Length > 500)
            throw new ArgumentException("Las observaciones no pueden exceder 500 caracteres.", nameof(observaciones));
    }
}
