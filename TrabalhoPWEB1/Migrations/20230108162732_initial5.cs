using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrabalhoPWEB1.Migrations
{
    public partial class initial5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "veiculoEntregue",
                table: "Reservas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "veiculoRecebido",
                table: "Reservas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "veiculoEntregue",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "veiculoRecebido",
                table: "Reservas");
        }
    }
}
