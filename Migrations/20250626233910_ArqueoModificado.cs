using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FondoUnicoSistemaCompleto.Migrations
{
    /// <inheritdoc />
    public partial class ArqueoModificado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Valor",
                table: "Arqueos",
                newName: "ValorEntregado");

            migrationBuilder.RenameColumn(
                name: "CantidadUtilizada",
                table: "Arqueos",
                newName: "cantidadDepositos");

            migrationBuilder.AlterColumn<float>(
                name: "TotalSobrante",
                table: "Arqueos",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ArqueoAnteriorCantidad",
                table: "Arqueos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "ArqueoAnteriorImporte",
                table: "Arqueos",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "CantidadADepositar",
                table: "Arqueos",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "CantidadRestante",
                table: "Arqueos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Coincidente",
                table: "Arqueos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TotalEntregadoImporte",
                table: "Arqueos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArqueoAnteriorCantidad",
                table: "Arqueos");

            migrationBuilder.DropColumn(
                name: "ArqueoAnteriorImporte",
                table: "Arqueos");

            migrationBuilder.DropColumn(
                name: "CantidadADepositar",
                table: "Arqueos");

            migrationBuilder.DropColumn(
                name: "CantidadRestante",
                table: "Arqueos");

            migrationBuilder.DropColumn(
                name: "Coincidente",
                table: "Arqueos");

            migrationBuilder.DropColumn(
                name: "TotalEntregadoImporte",
                table: "Arqueos");

            migrationBuilder.RenameColumn(
                name: "cantidadDepositos",
                table: "Arqueos",
                newName: "CantidadUtilizada");

            migrationBuilder.RenameColumn(
                name: "ValorEntregado",
                table: "Arqueos",
                newName: "Valor");

            migrationBuilder.AlterColumn<int>(
                name: "TotalSobrante",
                table: "Arqueos",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
