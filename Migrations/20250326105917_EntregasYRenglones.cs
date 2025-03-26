using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FondoUnicoSistemaCompleto.Migrations
{
    /// <inheritdoc />
    public partial class EntregasYRenglones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "RenglonesEntregas");

            migrationBuilder.DropTable(
                name: "Entregas");
        }
    }
}
