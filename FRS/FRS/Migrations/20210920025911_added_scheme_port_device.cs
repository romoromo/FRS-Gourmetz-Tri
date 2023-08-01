using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_scheme_port_device : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Port",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Scheme",
                table: "Devices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Port",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Scheme",
                table: "Devices");
        }
    }
}
