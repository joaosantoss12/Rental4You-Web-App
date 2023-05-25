using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrabalhoPWEB1.Migrations
{
    public partial class initial6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "danosVeiculos",
                table: "Reservas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "nKm",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "obs",
                table: "Reservas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "danosVeiculos",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "nKm",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "obs",
                table: "Reservas");
        }
    }
}
