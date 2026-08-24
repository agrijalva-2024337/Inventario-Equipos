using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventarioEquipos.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InicialNucleoMultiempresa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empresa",
                columns: table => new
                {
                    id_empresa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    nit_codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresa", x => x.id_empresa);
                });

            migrationBuilder.CreateTable(
                name: "Pais",
                columns: table => new
                {
                    id_pais = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    codigo_iso2 = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    codigo_iso3 = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    codigo_telefonico = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    moneda_local = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pais", x => x.id_pais);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_completo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    usuario_login = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.id_usuario);
                });

            migrationBuilder.CreateTable(
                name: "Area",
                columns: table => new
                {
                    id_area = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_empresa = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.id_area);
                    table.ForeignKey(
                        name: "FK_Area_Empresa",
                        column: x => x.id_empresa,
                        principalTable: "Empresa",
                        principalColumn: "id_empresa",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Categoria_Activo",
                columns: table => new
                {
                    id_categoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_empresa = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria_Activo", x => x.id_categoria);
                    table.ForeignKey(
                        name: "FK_Categoria_Empresa",
                        column: x => x.id_empresa,
                        principalTable: "Empresa",
                        principalColumn: "id_empresa",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Estado_Activo",
                columns: table => new
                {
                    id_estado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_empresa = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estado_Activo", x => x.id_estado);
                    table.ForeignKey(
                        name: "FK_EstadoActivo_Empresa",
                        column: x => x.id_empresa,
                        principalTable: "Empresa",
                        principalColumn: "id_empresa",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Motivo_Baja",
                columns: table => new
                {
                    id_motivo_baja = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_empresa = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motivo_Baja", x => x.id_motivo_baja);
                    table.ForeignKey(
                        name: "FK_MotivoBaja_Empresa",
                        column: x => x.id_empresa,
                        principalTable: "Empresa",
                        principalColumn: "id_empresa",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Proveedor",
                columns: table => new
                {
                    id_proveedor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_empresa = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    nit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    contacto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    telefono = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedor", x => x.id_proveedor);
                    table.ForeignKey(
                        name: "FK_Proveedor_Empresa",
                        column: x => x.id_empresa,
                        principalTable: "Empresa",
                        principalColumn: "id_empresa",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tipo_Mantenimiento",
                columns: table => new
                {
                    id_tipo_mantenimiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_empresa = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipo_Mantenimiento", x => x.id_tipo_mantenimiento);
                    table.ForeignKey(
                        name: "FK_TipoMantenimiento_Empresa",
                        column: x => x.id_empresa,
                        principalTable: "Empresa",
                        principalColumn: "id_empresa",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sede",
                columns: table => new
                {
                    id_sede = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_empresa = table.Column<int>(type: "int", nullable: false),
                    id_pais = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ciudad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sede", x => x.id_sede);
                    table.ForeignKey(
                        name: "FK_Sede_Empresa",
                        column: x => x.id_empresa,
                        principalTable: "Empresa",
                        principalColumn: "id_empresa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sede_Pais",
                        column: x => x.id_pais,
                        principalTable: "Pais",
                        principalColumn: "id_pais",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Historial_Cambios",
                columns: table => new
                {
                    id_historial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    id_empresa = table.Column<int>(type: "int", nullable: false),
                    fecha_hora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    tipo_operacion = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    entidad_afectada = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    id_registro_afectado = table.Column<int>(type: "int", nullable: false),
                    informacion_anterior = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    informacion_nueva = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historial_Cambios", x => x.id_historial);
                    table.ForeignKey(
                        name: "FK_HistorialCambios_Empresa",
                        column: x => x.id_empresa,
                        principalTable: "Empresa",
                        principalColumn: "id_empresa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialCambios_Usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuario_Empresa",
                columns: table => new
                {
                    id_usuario_empresa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    id_empresa = table.Column<int>(type: "int", nullable: false),
                    rol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    empresa_predeterminada = table.Column<bool>(type: "bit", nullable: false),
                    fecha_asignacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario_Empresa", x => x.id_usuario_empresa);
                    table.ForeignKey(
                        name: "FK_UsuarioEmpresa_Empresa",
                        column: x => x.id_empresa,
                        principalTable: "Empresa",
                        principalColumn: "id_empresa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioEmpresa_Usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Responsable",
                columns: table => new
                {
                    id_responsable = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_empresa = table.Column<int>(type: "int", nullable: false),
                    id_area = table.Column<int>(type: "int", nullable: false),
                    id_usuario = table.Column<int>(type: "int", nullable: true),
                    nombre_completo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    cargo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responsable", x => x.id_responsable);
                    table.ForeignKey(
                        name: "FK_Responsable_Area",
                        column: x => x.id_area,
                        principalTable: "Area",
                        principalColumn: "id_area",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Responsable_Empresa",
                        column: x => x.id_empresa,
                        principalTable: "Empresa",
                        principalColumn: "id_empresa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Responsable_Usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ubicacion",
                columns: table => new
                {
                    id_ubicacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_sede = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ubicacion", x => x.id_ubicacion);
                    table.ForeignKey(
                        name: "FK_Ubicacion_Sede",
                        column: x => x.id_sede,
                        principalTable: "Sede",
                        principalColumn: "id_sede",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Activo",
                columns: table => new
                {
                    id_activo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_empresa = table.Column<int>(type: "int", nullable: false),
                    codigo_interno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    id_categoria = table.Column<int>(type: "int", nullable: false),
                    marca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    modelo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    numero_serie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    fecha_compra = table.Column<DateTime>(type: "datetime2", nullable: true),
                    costo_adquisicion = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    moneda = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    id_proveedor = table.Column<int>(type: "int", nullable: true),
                    numero_factura = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    fecha_vencimiento_garantia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    id_sede = table.Column<int>(type: "int", nullable: false),
                    id_ubicacion = table.Column<int>(type: "int", nullable: false),
                    id_area = table.Column<int>(type: "int", nullable: true),
                    id_responsable = table.Column<int>(type: "int", nullable: true),
                    id_estado = table.Column<int>(type: "int", nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activo", x => x.id_activo);
                    table.ForeignKey(
                        name: "FK_Activo_Area",
                        column: x => x.id_area,
                        principalTable: "Area",
                        principalColumn: "id_area",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activo_Categoria",
                        column: x => x.id_categoria,
                        principalTable: "Categoria_Activo",
                        principalColumn: "id_categoria",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activo_Empresa",
                        column: x => x.id_empresa,
                        principalTable: "Empresa",
                        principalColumn: "id_empresa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activo_Estado",
                        column: x => x.id_estado,
                        principalTable: "Estado_Activo",
                        principalColumn: "id_estado",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activo_Proveedor",
                        column: x => x.id_proveedor,
                        principalTable: "Proveedor",
                        principalColumn: "id_proveedor",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activo_Responsable",
                        column: x => x.id_responsable,
                        principalTable: "Responsable",
                        principalColumn: "id_responsable",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activo_Sede",
                        column: x => x.id_sede,
                        principalTable: "Sede",
                        principalColumn: "id_sede",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activo_Ubicacion",
                        column: x => x.id_ubicacion,
                        principalTable: "Ubicacion",
                        principalColumn: "id_ubicacion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inventario_Fisico",
                columns: table => new
                {
                    id_inventario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_empresa = table.Column<int>(type: "int", nullable: false),
                    id_sede = table.Column<int>(type: "int", nullable: true),
                    id_ubicacion = table.Column<int>(type: "int", nullable: true),
                    fecha_inicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_cierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    id_usuario_responsable = table.Column<int>(type: "int", nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventario_Fisico", x => x.id_inventario);
                    table.ForeignKey(
                        name: "FK_InventarioFisico_Empresa",
                        column: x => x.id_empresa,
                        principalTable: "Empresa",
                        principalColumn: "id_empresa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventarioFisico_Sede",
                        column: x => x.id_sede,
                        principalTable: "Sede",
                        principalColumn: "id_sede",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventarioFisico_Ubicacion",
                        column: x => x.id_ubicacion,
                        principalTable: "Ubicacion",
                        principalColumn: "id_ubicacion",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventarioFisico_Usuario",
                        column: x => x.id_usuario_responsable,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Asignacion",
                columns: table => new
                {
                    id_asignacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_activo = table.Column<int>(type: "int", nullable: false),
                    id_responsable = table.Column<int>(type: "int", nullable: false),
                    id_ubicacion_uso = table.Column<int>(type: "int", nullable: false),
                    fecha_asignacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    entregado_por = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    recibido_por = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    fecha_devolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    activa = table.Column<bool>(type: "bit", nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asignacion", x => x.id_asignacion);
                    table.ForeignKey(
                        name: "FK_Asignacion_Activo",
                        column: x => x.id_activo,
                        principalTable: "Activo",
                        principalColumn: "id_activo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asignacion_Responsable",
                        column: x => x.id_responsable,
                        principalTable: "Responsable",
                        principalColumn: "id_responsable",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asignacion_Ubicacion",
                        column: x => x.id_ubicacion_uso,
                        principalTable: "Ubicacion",
                        principalColumn: "id_ubicacion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Baja",
                columns: table => new
                {
                    id_baja = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_activo = table.Column<int>(type: "int", nullable: false),
                    id_motivo_baja = table.Column<int>(type: "int", nullable: false),
                    fecha_baja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    documento_referencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    autorizado_por = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    observaciones = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baja", x => x.id_baja);
                    table.ForeignKey(
                        name: "FK_Baja_Activo",
                        column: x => x.id_activo,
                        principalTable: "Activo",
                        principalColumn: "id_activo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Baja_MotivoBaja",
                        column: x => x.id_motivo_baja,
                        principalTable: "Motivo_Baja",
                        principalColumn: "id_motivo_baja",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Mantenimiento",
                columns: table => new
                {
                    id_mantenimiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_activo = table.Column<int>(type: "int", nullable: false),
                    id_tipo_mantenimiento = table.Column<int>(type: "int", nullable: false),
                    id_proveedor = table.Column<int>(type: "int", nullable: true),
                    fecha_programada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_realizado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    responsable = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    descripcion_problema = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    trabajo_realizado = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    costo = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    numero_factura = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    estado_mantenimiento = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantenimiento", x => x.id_mantenimiento);
                    table.ForeignKey(
                        name: "FK_Mantenimiento_Activo",
                        column: x => x.id_activo,
                        principalTable: "Activo",
                        principalColumn: "id_activo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mantenimiento_Proveedor",
                        column: x => x.id_proveedor,
                        principalTable: "Proveedor",
                        principalColumn: "id_proveedor",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mantenimiento_Tipo",
                        column: x => x.id_tipo_mantenimiento,
                        principalTable: "Tipo_Mantenimiento",
                        principalColumn: "id_tipo_mantenimiento",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Traslado",
                columns: table => new
                {
                    id_traslado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_activo = table.Column<int>(type: "int", nullable: false),
                    id_ubicacion_origen = table.Column<int>(type: "int", nullable: false),
                    id_ubicacion_destino = table.Column<int>(type: "int", nullable: false),
                    fecha_traslado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    motivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    responsable_traslado = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Traslado", x => x.id_traslado);
                    table.ForeignKey(
                        name: "FK_Traslado_Activo",
                        column: x => x.id_activo,
                        principalTable: "Activo",
                        principalColumn: "id_activo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Traslado_Destino",
                        column: x => x.id_ubicacion_destino,
                        principalTable: "Ubicacion",
                        principalColumn: "id_ubicacion",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Traslado_Origen",
                        column: x => x.id_ubicacion_origen,
                        principalTable: "Ubicacion",
                        principalColumn: "id_ubicacion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Detalle_Inventario",
                columns: table => new
                {
                    id_detalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_inventario = table.Column<int>(type: "int", nullable: false),
                    id_activo = table.Column<int>(type: "int", nullable: false),
                    encontrado = table.Column<bool>(type: "bit", nullable: false),
                    id_ubicacion_encontrada = table.Column<int>(type: "int", nullable: true),
                    estado_fisico_observado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    observaciones = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    fecha_verificacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detalle_Inventario", x => x.id_detalle);
                    table.ForeignKey(
                        name: "FK_DetalleInventario_Activo",
                        column: x => x.id_activo,
                        principalTable: "Activo",
                        principalColumn: "id_activo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleInventario_Inventario",
                        column: x => x.id_inventario,
                        principalTable: "Inventario_Fisico",
                        principalColumn: "id_inventario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleInventario_Ubicacion",
                        column: x => x.id_ubicacion_encontrada,
                        principalTable: "Ubicacion",
                        principalColumn: "id_ubicacion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activo_id_area",
                table: "Activo",
                column: "id_area");

            migrationBuilder.CreateIndex(
                name: "IX_Activo_id_categoria",
                table: "Activo",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_Activo_id_estado",
                table: "Activo",
                column: "id_estado");

            migrationBuilder.CreateIndex(
                name: "IX_Activo_id_proveedor",
                table: "Activo",
                column: "id_proveedor");

            migrationBuilder.CreateIndex(
                name: "IX_Activo_id_responsable",
                table: "Activo",
                column: "id_responsable");

            migrationBuilder.CreateIndex(
                name: "IX_Activo_id_sede",
                table: "Activo",
                column: "id_sede");

            migrationBuilder.CreateIndex(
                name: "IX_Activo_id_ubicacion",
                table: "Activo",
                column: "id_ubicacion");

            migrationBuilder.CreateIndex(
                name: "UQ_Activo_CodigoInterno",
                table: "Activo",
                columns: new[] { "id_empresa", "codigo_interno" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Area_id_empresa",
                table: "Area",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_Asignacion_id_responsable",
                table: "Asignacion",
                column: "id_responsable");

            migrationBuilder.CreateIndex(
                name: "IX_Asignacion_id_ubicacion_uso",
                table: "Asignacion",
                column: "id_ubicacion_uso");

            migrationBuilder.CreateIndex(
                name: "UQ_Asignacion_Activo_Activa",
                table: "Asignacion",
                column: "id_activo",
                unique: true,
                filter: "[activa] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Baja_id_activo",
                table: "Baja",
                column: "id_activo");

            migrationBuilder.CreateIndex(
                name: "IX_Baja_id_motivo_baja",
                table: "Baja",
                column: "id_motivo_baja");

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_Activo_id_empresa",
                table: "Categoria_Activo",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Inventario_id_activo",
                table: "Detalle_Inventario",
                column: "id_activo");

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Inventario_id_inventario",
                table: "Detalle_Inventario",
                column: "id_inventario");

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Inventario_id_ubicacion_encontrada",
                table: "Detalle_Inventario",
                column: "id_ubicacion_encontrada");

            migrationBuilder.CreateIndex(
                name: "UQ_Empresa_Nit",
                table: "Empresa",
                column: "nit_codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estado_Activo_id_empresa",
                table: "Estado_Activo",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_Historial_Cambios_id_empresa",
                table: "Historial_Cambios",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_Historial_Cambios_id_usuario",
                table: "Historial_Cambios",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Inventario_Fisico_id_empresa",
                table: "Inventario_Fisico",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_Inventario_Fisico_id_sede",
                table: "Inventario_Fisico",
                column: "id_sede");

            migrationBuilder.CreateIndex(
                name: "IX_Inventario_Fisico_id_ubicacion",
                table: "Inventario_Fisico",
                column: "id_ubicacion");

            migrationBuilder.CreateIndex(
                name: "IX_Inventario_Fisico_id_usuario_responsable",
                table: "Inventario_Fisico",
                column: "id_usuario_responsable");

            migrationBuilder.CreateIndex(
                name: "IX_Mantenimiento_id_activo",
                table: "Mantenimiento",
                column: "id_activo");

            migrationBuilder.CreateIndex(
                name: "IX_Mantenimiento_id_proveedor",
                table: "Mantenimiento",
                column: "id_proveedor");

            migrationBuilder.CreateIndex(
                name: "IX_Mantenimiento_id_tipo_mantenimiento",
                table: "Mantenimiento",
                column: "id_tipo_mantenimiento");

            migrationBuilder.CreateIndex(
                name: "IX_Motivo_Baja_id_empresa",
                table: "Motivo_Baja",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_Pais_codigo_iso2",
                table: "Pais",
                column: "codigo_iso2",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pais_codigo_iso3",
                table: "Pais",
                column: "codigo_iso3",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proveedor_id_empresa",
                table: "Proveedor",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_Responsable_id_area",
                table: "Responsable",
                column: "id_area");

            migrationBuilder.CreateIndex(
                name: "IX_Responsable_id_empresa",
                table: "Responsable",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_Responsable_id_usuario",
                table: "Responsable",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Sede_id_empresa",
                table: "Sede",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_Sede_id_pais",
                table: "Sede",
                column: "id_pais");

            migrationBuilder.CreateIndex(
                name: "IX_Tipo_Mantenimiento_id_empresa",
                table: "Tipo_Mantenimiento",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "IX_Traslado_id_activo",
                table: "Traslado",
                column: "id_activo");

            migrationBuilder.CreateIndex(
                name: "IX_Traslado_id_ubicacion_destino",
                table: "Traslado",
                column: "id_ubicacion_destino");

            migrationBuilder.CreateIndex(
                name: "IX_Traslado_id_ubicacion_origen",
                table: "Traslado",
                column: "id_ubicacion_origen");

            migrationBuilder.CreateIndex(
                name: "IX_Ubicacion_id_sede",
                table: "Ubicacion",
                column: "id_sede");

            migrationBuilder.CreateIndex(
                name: "UQ_Usuario_Correo",
                table: "Usuario",
                column: "correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Usuario_Login",
                table: "Usuario",
                column: "usuario_login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Empresa_id_empresa",
                table: "Usuario_Empresa",
                column: "id_empresa");

            migrationBuilder.CreateIndex(
                name: "UQ_UsuarioEmpresa",
                table: "Usuario_Empresa",
                columns: new[] { "id_usuario", "id_empresa" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asignacion");

            migrationBuilder.DropTable(
                name: "Baja");

            migrationBuilder.DropTable(
                name: "Detalle_Inventario");

            migrationBuilder.DropTable(
                name: "Historial_Cambios");

            migrationBuilder.DropTable(
                name: "Mantenimiento");

            migrationBuilder.DropTable(
                name: "Traslado");

            migrationBuilder.DropTable(
                name: "Usuario_Empresa");

            migrationBuilder.DropTable(
                name: "Motivo_Baja");

            migrationBuilder.DropTable(
                name: "Inventario_Fisico");

            migrationBuilder.DropTable(
                name: "Tipo_Mantenimiento");

            migrationBuilder.DropTable(
                name: "Activo");

            migrationBuilder.DropTable(
                name: "Categoria_Activo");

            migrationBuilder.DropTable(
                name: "Estado_Activo");

            migrationBuilder.DropTable(
                name: "Proveedor");

            migrationBuilder.DropTable(
                name: "Responsable");

            migrationBuilder.DropTable(
                name: "Ubicacion");

            migrationBuilder.DropTable(
                name: "Area");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Sede");

            migrationBuilder.DropTable(
                name: "Empresa");

            migrationBuilder.DropTable(
                name: "Pais");
        }
    }
}
