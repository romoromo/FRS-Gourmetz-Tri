using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addEmergencyMessageToDevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmergencyMessage1",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyMessage2",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyMessage3",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyMessage4",
                table: "Devices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmergencyMessage1",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "EmergencyMessage2",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "EmergencyMessage3",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "EmergencyMessage4",
                table: "Devices");
        }
    }
}
