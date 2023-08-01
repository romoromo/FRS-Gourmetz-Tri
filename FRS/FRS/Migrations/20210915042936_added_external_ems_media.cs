using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_external_ems_media : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "external_ems_id",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "external_media_id",
                table: "Devices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "external_ems_id",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "external_media_id",
                table: "Devices");
        }
    }
}
