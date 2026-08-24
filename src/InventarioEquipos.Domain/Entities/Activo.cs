using InventarioEquipos.Domain.Common;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Activo físico de una empresa: el corazón del sistema de inventario.
/// Depende de Empresa, CategoriaActivo, Sede, Ubicacion y EstadoActivo
/// (todas obligatorias) y de Proveedor, Area y Responsable (opcionales).
/// Columnas según el diagrama: id_empresa, codigo_interno, nombre,
/// descripcion, id_categoria, marca, modelo, numero_serie, fecha_compra,
/// costo_adquisicion, moneda, id_proveedor, numero_factura,
/// fecha_vencimiento_garantia, id_sede, id_ubicacion, id_area,
/// id_responsable, id_estado, observaciones.
///
/// Las FKs se nombran IdEmpresa / IdCategoria / IdProveedor / IdSede /
/// IdUbicacion / IdArea / IdResponsable / IdEstado para que coincidan
/// con id_empresa / id_categoria / id_proveedor / id_sede / id_ubicacion
/// / id_area / id_responsable / id_estado al pasar a snake_case.
///
/// Reglas de negocio del DERCAS que NO se validan aquí porque cruzan
/// varias filas / varias entidades (van en Application):
/// - "El código interno del activo deberá ser único dentro de la
///   empresa": se garantiza con un índice único compuesto
///   (IdEmpresa, CodigoInterno) en la configuración de EF Core.
/// - "Un activo dado de baja no podrá asignarse, trasladarse ni enviarse
///   a mantenimiento": requiere conocer el catálogo EstadoActivo vigente,
///   se valida en el servicio de aplicación antes de crear una Asignacion,
///   Traslado o Mantenimiento.
/// </summary>
public class Activo : EntityBase
{
    private const decimal CostoMaximo = 9_999_999_999.99m;

    public int IdEmpresa { get; private set; }
    public Empresa? Empresa { get; private set; }

    public string CodigoInterno { get; private set; } = default!;
    public string Nombre { get; private set; } = default!;
    public string Descripcion { get; private set; } = default!;

    public int IdCategoria { get; private set; }
    public CategoriaActivo? CategoriaActivo { get; private set; }

    public string Marca { get; private set; } = default!;
    public string Modelo { get; private set; } = default!;
    public string NumeroSerie { get; private set; } = default!;

    public DateTime? FechaCompra { get; private set; }
    public decimal CostoAdquisicion { get; private set; }
    public string? Moneda { get; private set; }

    public int? IdProveedor { get; private set; }
    public Proveedor? Proveedor { get; private set; }

    public string? NumeroFactura { get; private set; }
    public DateTime? FechaVencimientoGarantia { get; private set; }

    public int IdSede { get; private set; }
    public Sede? Sede { get; private set; }

    /// <summary>Ubicación actual del activo (única en un momento dado).</summary>
    public int IdUbicacion { get; private set; }
    public Ubicacion? Ubicacion { get; private set; }

    public int? IdArea { get; private set; }
    public Area? Area { get; private set; }

    /// <summary>Responsable actual del activo; null si todavía no se ha asignado a nadie.</summary>
    public int? IdResponsable { get; private set; }
    public Responsable? Responsable { get; private set; }

    public int IdEstado { get; private set; }
    public EstadoActivo? EstadoActivo { get; private set; }

    public string? Observaciones { get; private set; }

    protected Activo() { }

    private Activo(
        int idEmpresa,
        string codigoInterno,
        string nombre,
        string descripcion,
        int idCategoria,
        string marca,
        string modelo,
        string numeroSerie,
        DateTime? fechaCompra,
        decimal costoAdquisicion,
        string? moneda,
        int? idProveedor,
        string? numeroFactura,
        DateTime? fechaVencimientoGarantia,
        int idSede,
        int idUbicacion,
        int? idArea,
        int? idResponsable,
        int idEstado,
        string? observaciones)
    {
        IdEmpresa = idEmpresa;
        CodigoInterno = codigoInterno;
        Nombre = nombre;
        Descripcion = descripcion;
        IdCategoria = idCategoria;
        Marca = marca;
        Modelo = modelo;
        NumeroSerie = numeroSerie;
        FechaCompra = fechaCompra;
        CostoAdquisicion = costoAdquisicion;
        Moneda = moneda;
        IdProveedor = idProveedor;
        NumeroFactura = numeroFactura;
        FechaVencimientoGarantia = fechaVencimientoGarantia;
        IdSede = idSede;
        IdUbicacion = idUbicacion;
        IdArea = idArea;
        IdResponsable = idResponsable;
        IdEstado = idEstado;
        Observaciones = observaciones;
    }

    public static Activo Crear(
        int idEmpresa,
        string codigoInterno,
        string nombre,
        string descripcion,
        int idCategoria,
        string marca,
        string modelo,
        string numeroSerie,
        decimal costoAdquisicion,
        int idSede,
        int idUbicacion,
        int idEstado,
        DateTime? fechaCompra = null,
        string? moneda = null,
        int? idProveedor = null,
        string? numeroFactura = null,
        DateTime? fechaVencimientoGarantia = null,
        int? idArea = null,
        int? idResponsable = null,
        string? observaciones = null)
    {
        ValidarIdEmpresa(idEmpresa);
        ValidarCodigoInterno(codigoInterno);
        ValidarNombre(nombre);
        ValidarDescripcion(descripcion);
        ValidarIdCategoria(idCategoria);
        ValidarMarca(marca);
        ValidarModelo(modelo);
        ValidarNumeroSerie(numeroSerie);
        ValidarCostoAdquisicion(costoAdquisicion);
        ValidarIdSede(idSede);
        ValidarIdUbicacion(idUbicacion);
        ValidarIdEstado(idEstado);
        ValidarIdProveedor(idProveedor);
        ValidarIdArea(idArea);
        ValidarIdResponsable(idResponsable);
        ValidarMoneda(moneda);
        ValidarNumeroFactura(numeroFactura);
        ValidarObservaciones(observaciones);

        return new Activo(
            idEmpresa,
            codigoInterno.Trim(),
            nombre.Trim(),
            descripcion.Trim(),
            idCategoria,
            marca.Trim(),
            modelo.Trim(),
            numeroSerie.Trim(),
            fechaCompra,
            costoAdquisicion,
            moneda?.Trim().ToUpperInvariant(),
            idProveedor,
            numeroFactura?.Trim(),
            fechaVencimientoGarantia,
            idSede,
            idUbicacion,
            idArea,
            idResponsable,
            idEstado,
            observaciones?.Trim());
    }

    public void ActualizarDatos(
        string codigoInterno,
        string nombre,
        string descripcion,
        int idCategoria,
        string marca,
        string modelo,
        string numeroSerie,
        decimal costoAdquisicion,
        DateTime? fechaCompra,
        string? moneda,
        int? idProveedor,
        string? numeroFactura,
        DateTime? fechaVencimientoGarantia,
        string? observaciones)
    {
        ValidarCodigoInterno(codigoInterno);
        ValidarNombre(nombre);
        ValidarDescripcion(descripcion);
        ValidarIdCategoria(idCategoria);
        ValidarMarca(marca);
        ValidarModelo(modelo);
        ValidarNumeroSerie(numeroSerie);
        ValidarCostoAdquisicion(costoAdquisicion);
        ValidarIdProveedor(idProveedor);
        ValidarMoneda(moneda);
        ValidarNumeroFactura(numeroFactura);
        ValidarObservaciones(observaciones);

        CodigoInterno = codigoInterno.Trim();
        Nombre = nombre.Trim();
        Descripcion = descripcion.Trim();
        IdCategoria = idCategoria;
        Marca = marca.Trim();
        Modelo = modelo.Trim();
        NumeroSerie = numeroSerie.Trim();
        FechaCompra = fechaCompra;
        CostoAdquisicion = costoAdquisicion;
        Moneda = moneda?.Trim().ToUpperInvariant();
        IdProveedor = idProveedor;
        NumeroFactura = numeroFactura?.Trim();
        FechaVencimientoGarantia = fechaVencimientoGarantia;
        Observaciones = observaciones?.Trim();
    }

    /// <summary>Usado por Traslados: cambia la ubicación (y opcionalmente sede/área) actual.</summary>
    public void ActualizarUbicacionActual(int idSede, int idUbicacion, int? idArea)
    {
        ValidarIdSede(idSede);
        ValidarIdUbicacion(idUbicacion);
        ValidarIdArea(idArea);

        IdSede = idSede;
        IdUbicacion = idUbicacion;
        IdArea = idArea;
    }

    /// <summary>Usado por Asignaciones: cambia quién es el responsable actual (null = sin asignar).</summary>
    public void ActualizarResponsableActual(int? idResponsable)
    {
        ValidarIdResponsable(idResponsable);
        IdResponsable = idResponsable;
    }

    /// <summary>
    /// Cambia el estado del activo dentro del catálogo EstadoActivo de su
    /// empresa (ej. "Disponible", "Asignado", "En mantenimiento", "De baja").
    /// No existe un booleano Activo/Inactivo separado: el estado del activo
    /// siempre es uno de los valores de ese catálogo.
    /// </summary>
    public void CambiarEstado(int idEstado)
    {
        ValidarIdEstado(idEstado);
        IdEstado = idEstado;
    }

    private static void ValidarIdEmpresa(int idEmpresa)
    {
        if (idEmpresa <= 0)
            throw new ArgumentException("La empresa es obligatoria.", nameof(idEmpresa));
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

    private static void ValidarDescripcion(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción es obligatoria.", nameof(descripcion));
        if (descripcion.Trim().Length > 300)
            throw new ArgumentException("La descripción no puede exceder 300 caracteres.", nameof(descripcion));
    }

    private static void ValidarIdCategoria(int idCategoria)
    {
        if (idCategoria <= 0)
            throw new ArgumentException("La categoría es obligatoria.", nameof(idCategoria));
    }

    private static void ValidarMarca(string marca)
    {
        if (string.IsNullOrWhiteSpace(marca))
            throw new ArgumentException("La marca es obligatoria.", nameof(marca));
        if (marca.Trim().Length > 100)
            throw new ArgumentException("La marca no puede exceder 100 caracteres.", nameof(marca));
    }

    private static void ValidarModelo(string modelo)
    {
        if (string.IsNullOrWhiteSpace(modelo))
            throw new ArgumentException("El modelo es obligatorio.", nameof(modelo));
        if (modelo.Trim().Length > 100)
            throw new ArgumentException("El modelo no puede exceder 100 caracteres.", nameof(modelo));
    }

    private static void ValidarNumeroSerie(string numeroSerie)
    {
        if (string.IsNullOrWhiteSpace(numeroSerie))
            throw new ArgumentException("El número de serie es obligatorio.", nameof(numeroSerie));
        if (numeroSerie.Trim().Length > 100)
            throw new ArgumentException("El número de serie no puede exceder 100 caracteres.", nameof(numeroSerie));
    }

    private static void ValidarCostoAdquisicion(decimal costoAdquisicion)
    {
        if (costoAdquisicion < 0)
            throw new ArgumentException("El costo de adquisición no puede ser negativo.", nameof(costoAdquisicion));
        if (costoAdquisicion > CostoMaximo)
            throw new ArgumentException("El costo de adquisición no puede exceder 9,999,999,999.99.", nameof(costoAdquisicion));
        if (decimal.Round(costoAdquisicion, 2) != costoAdquisicion)
            throw new ArgumentException("El costo de adquisición no puede tener más de 2 decimales.", nameof(costoAdquisicion));
    }

    private static void ValidarIdSede(int idSede)
    {
        if (idSede <= 0)
            throw new ArgumentException("La sede es obligatoria.", nameof(idSede));
    }

    private static void ValidarIdUbicacion(int idUbicacion)
    {
        if (idUbicacion <= 0)
            throw new ArgumentException("La ubicación es obligatoria.", nameof(idUbicacion));
    }

    private static void ValidarIdArea(int? idArea)
    {
        if (idArea is <= 0)
            throw new ArgumentException("El área, si se informa, debe ser mayor a 0.", nameof(idArea));
    }

    private static void ValidarIdEstado(int idEstado)
    {
        if (idEstado <= 0)
            throw new ArgumentException("El estado del activo es obligatorio.", nameof(idEstado));
    }

    private static void ValidarIdProveedor(int? idProveedor)
    {
        if (idProveedor is <= 0)
            throw new ArgumentException("El proveedor, si se informa, debe ser mayor a 0.", nameof(idProveedor));
    }

    private static void ValidarIdResponsable(int? idResponsable)
    {
        if (idResponsable is <= 0)
            throw new ArgumentException("El responsable, si se informa, debe ser mayor a 0.", nameof(idResponsable));
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
