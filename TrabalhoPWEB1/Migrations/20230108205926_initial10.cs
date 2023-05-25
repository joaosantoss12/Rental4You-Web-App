using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrabalhoPWEB1.Migrations
{
    public partial class initial10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "obsCliente",
                table: "Reservas",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "obsCliente",
                table: "Reservas");
        }
    }
}
