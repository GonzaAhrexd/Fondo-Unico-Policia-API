using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FondoUnicoSistemaCompleto.Migrations
{
    /// <inheritdoc />
    public partial class MigracionReiniciada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bancos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Banco = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bancos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Depositos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NroDeposito = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodoArqueo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Banco = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cuenta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoFormulario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Boleta = table.Column<int>(type: "int", nullable: false),
                    Importe = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Depositos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Entregas",
                columns: table => new
                {
                    NroEntrega = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NroEntregaManual = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    estaActivo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entregas", x => x.NroEntrega);
                });

            migrationBuilder.CreateTable(
                name: "Formularios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Formulario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Importe = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formularios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Localidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoPostal = table.Column<int>(type: "int", nullable: false),
                    Localidad = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistroEntregas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoEntrega = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cantidadActual = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroEntregas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Unidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Verificaciones = table.Column<bool>(type: "bit", nullable: false),
                    FondoUnico = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Usuario_repo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre_de_usuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DNI = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Verificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recibo = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Responsable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Formulario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Anio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dominio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Importe = table.Column<double>(type: "float", nullable: false),
                    estaActivo = table.Column<bool>(type: "bit", nullable: false),
                    estaAnulado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verificaciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RenglonesEntregas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NroRenglon = table.Column<int>(type: "int", nullable: false),
                    TipoFormulario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    desde = table.Column<int>(type: "int", nullable: false),
                    hasta = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    EntregasNroEntrega = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RenglonesEntregas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RenglonesEntregas_Entregas_EntregasNroEntrega",
                        column: x => x.EntregasNroEntrega,
                        principalTable: "Entregas",
                        principalColumn: "NroEntrega");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RenglonesEntregas_EntregasNroEntrega",
                table: "RenglonesEntregas",
                column: "EntregasNroEntrega");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bancos");

            migrationBuilder.DropTable(
                name: "Depositos");

            migrationBuilder.DropTable(
                name: "Formularios");

            migrationBuilder.DropTable(
                name: "Localidades");

            migrationBuilder.DropTable(
                name: "RegistroEntregas");

            migrationBuilder.DropTable(
                name: "RenglonesEntregas");

            migrationBuilder.DropTable(
                name: "Unidades");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Verificaciones");

            migrationBuilder.DropTable(
                name: "Entregas");
        }
    }
}
