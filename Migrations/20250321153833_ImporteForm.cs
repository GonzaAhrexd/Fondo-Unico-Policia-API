using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FondoUnicoSistemaCompleto.Migrations
{
    /// <inheritdoc />
    public partial class ImporteForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Valor",
                table: "Formularios",
                newName: "Importe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Importe",
                table: "Formularios",
                newName: "Valor");
        }
    }
}
