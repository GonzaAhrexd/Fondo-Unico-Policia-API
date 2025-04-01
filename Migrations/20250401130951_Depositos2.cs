using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FondoUnicoSistemaCompleto.Migrations
{
    /// <inheritdoc />
    public partial class Depositos2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    Boleta = table.Column<int>(type: "int", nullable: false),
                    Importe = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Depositos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Depositos");
        }
    }
}
