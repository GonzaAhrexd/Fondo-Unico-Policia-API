using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FondoUnicoSistemaCompleto.Migrations
{
    /// <inheritdoc />
    public partial class RemovidoTipoFormulario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Formularios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Formularios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
