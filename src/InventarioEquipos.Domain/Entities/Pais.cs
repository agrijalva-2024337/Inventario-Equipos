using InventarioEquipos.Domain.Common;
using InventarioEquipos.Domain.Enums;

namespace InventarioEquipos.Domain.Entities;

/// <summary>
/// Catálogo de países. No depende de Empresa. Columnas según el diagrama:
/// nombre, codigo_iso2, codigo_iso3, codigo_telefonico, moneda_local, estado.
/// (El diagrama no incluye fecha_creacion para esta tabla.)
/// </summary>
public class Pais : EntityBase
{
    public string Nombre { get; private set; } = default!;
    public string CodigoIso2 { get; private set; } = default!;
    public string CodigoIso3 { get; private set; } = default!;
    public string CodigoTelefonico { get; private set; } = default!;
    public string MonedaLocal { get; private set; } = default!;
    public EstadoRegistro Estado { get; private set; }

    protected Pais() { }

    private Pais(
        string nombre,
        string codigoIso2,
        string codigoIso3,
        string codigoTelefonico,
        string monedaLocal)
    {
        Nombre = nombre;
        CodigoIso2 = codigoIso2;
        CodigoIso3 = codigoIso3;
        CodigoTelefonico = codigoTelefonico;
        MonedaLocal = monedaLocal;
        Estado = EstadoRegistro.Activo;
    }

    public static Pais Crear(
        string nombre,
        string codigoIso2,
        string codigoIso3,
        string codigoTelefonico,
        string monedaLocal)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del país es obligatorio.", nameof(nombre));
        if (nombre.Length > 100)
            throw new ArgumentException("El nombre del país no puede exceder 100 caracteres.", nameof(nombre));

        codigoIso2 = ValidarCodigoIso(codigoIso2, 2, nameof(codigoIso2));
        codigoIso3 = ValidarCodigoIso(codigoIso3, 3, nameof(codigoIso3));

        if (string.IsNullOrWhiteSpace(codigoTelefonico))
            throw new ArgumentException("El código telefónico es obligatorio.", nameof(codigoTelefonico));
        if (codigoTelefonico.Length > 10)
            throw new ArgumentException("El código telefónico no puede exceder 10 caracteres.", nameof(codigoTelefonico));
        if (string.IsNullOrWhiteSpace(monedaLocal))
            throw new ArgumentException("La moneda local es obligatoria.", nameof(monedaLocal));
        if (monedaLocal.Length > 10)
            throw new ArgumentException("La moneda local no puede exceder 10 caracteres.", nameof(monedaLocal));

        return new Pais(nombre.Trim(), codigoIso2, codigoIso3, codigoTelefonico.Trim(), monedaLocal.Trim().ToUpperInvariant());
    }

    public void ActualizarDatos(
        string nombre,
        string codigoIso2,
        string codigoIso3,
        string codigoTelefonico,
        string monedaLocal)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del país es obligatorio.", nameof(nombre));

        Nombre = nombre.Trim();
        CodigoIso2 = ValidarCodigoIso(codigoIso2, 2, nameof(codigoIso2));
        CodigoIso3 = ValidarCodigoIso(codigoIso3, 3, nameof(codigoIso3));
        CodigoTelefonico = codigoTelefonico.Trim();
        MonedaLocal = monedaLocal.Trim().ToUpperInvariant();
    }

    public void Activar() => Estado = EstadoRegistro.Activo;

    public void Desactivar() => Estado = EstadoRegistro.Inactivo;

    private static string ValidarCodigoIso(string codigo, int longitudEsperada, string paramName)
    {
        if (string.IsNullOrWhiteSpace(codigo) || codigo.Trim().Length != longitudEsperada)
            throw new ArgumentException($"El código ISO debe tener exactamente {longitudEsperada} caracteres.", paramName);

        return codigo.Trim().ToUpperInvariant();
    }
}
