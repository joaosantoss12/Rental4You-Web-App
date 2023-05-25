using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrabalhoPWEB1.Migrations
{
    public partial class initial7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nKm",
                table: "Reservas");

            migrationBuilder.AddColumn<int>(
                name: "nKm",
                table: "Veiculos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "emailFuncionario",
                table: "Reservas",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nKm",
                table: "Veiculos");

            migrationBuilder.DropColumn(
                name: "emailFuncionario",
                table: "Reservas");

            migrationBuilder.AddColumn<int>(
                name: "nKm",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
