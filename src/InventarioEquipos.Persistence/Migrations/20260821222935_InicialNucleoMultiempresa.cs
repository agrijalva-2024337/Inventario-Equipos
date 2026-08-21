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
                name: "empresas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    nit_codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    telefono = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_empresas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "paises",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    codigo_iso2 = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    codigo_iso3 = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    codigo_telefonico = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    moneda_local = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_paises", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("p_k_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sedes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    empresa_id = table.Column<int>(type: "int", nullable: false),
                    pais_id = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ciudad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_sedes", x => x.id);
                    table.ForeignKey(
                        name: "f_k_sedes_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_sedes_paises_pais_id",
                        column: x => x.pais_id,
                        principalTable: "paises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "historial_cambios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    usuario_id = table.Column<int>(type: "int", nullable: false),
                    empresa_id = table.Column<int>(type: "int", nullable: false),
                    fecha_hora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    tipo_operacion = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    entidad_afectada = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    id_registro_afectado = table.Column<int>(type: "int", nullable: false),
                    informacion_anterior = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    informacion_nueva = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_historial_cambios", x => x.id);
                    table.ForeignKey(
                        name: "f_k_historial_cambios__usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_historial_cambios_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usuario_empresas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    usuario_id = table.Column<int>(type: "int", nullable: false),
                    empresa_id = table.Column<int>(type: "int", nullable: false),
                    rol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    empresa_predeterminada = table.Column<bool>(type: "bit", nullable: false),
                    fecha_asignacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_usuario_empresas", x => x.id);
                    table.ForeignKey(
                        name: "f_k_usuario_empresas_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_usuario_empresas_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_empresas_nit_codigo",
                table: "empresas",
                column: "nit_codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_historial_cambios_empresa_id",
                table: "historial_cambios",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "i_x_historial_cambios_usuario_id",
                table: "historial_cambios",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "i_x_paises_codigo_iso2",
                table: "paises",
                column: "codigo_iso2",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_paises_codigo_iso3",
                table: "paises",
                column: "codigo_iso3",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_sedes_empresa_id",
                table: "sedes",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "i_x_sedes_pais_id",
                table: "sedes",
                column: "pais_id");

            migrationBuilder.CreateIndex(
                name: "i_x_usuario_empresas_empresa_id",
                table: "usuario_empresas",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "i_x_usuario_empresas_usuario_id_empresa_id",
                table: "usuario_empresas",
                columns: new[] { "usuario_id", "empresa_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_usuarios_correo",
                table: "usuarios",
                column: "correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_usuarios_usuario_login",
                table: "usuarios",
                column: "usuario_login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "historial_cambios");

            migrationBuilder.DropTable(
                name: "sedes");

            migrationBuilder.DropTable(
                name: "usuario_empresas");

            migrationBuilder.DropTable(
                name: "paises");

            migrationBuilder.DropTable(
                name: "empresas");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
