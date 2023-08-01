using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class updated_outletprofile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "OutletProfiles");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "OutletProfiles",
                newName: "Label");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Label",
                table: "OutletProfiles",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "OutletProfiles",
                nullable: true);
        }
    }
}
