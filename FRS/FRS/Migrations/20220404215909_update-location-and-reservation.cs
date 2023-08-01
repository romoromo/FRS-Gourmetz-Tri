using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class updatelocationandreservation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayTitle",
                table: "Reservations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeetingPurpose",
                table: "Reservations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusRemark",
                table: "Reservations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Access",
                table: "Locations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DirectoryId",
                table: "Locations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExchangeId",
                table: "Locations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "Locations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FloorId",
                table: "Locations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IP",
                table: "Locations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "Locations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "deviceName",
                table: "Locations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "floorX",
                table: "Locations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "floorY",
                table: "Locations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "remarks",
                table: "Locations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "seatingCapacity",
                table: "Locations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_DirectoryId",
                table: "Locations",
                column: "DirectoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_DirectoryListing_DirectoryId",
                table: "Locations",
                column: "DirectoryId",
                principalTable: "DirectoryListing",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_DirectoryListing_DirectoryId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Locations_DirectoryId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "DisplayTitle",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "MeetingPurpose",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "StatusRemark",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Access",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "DirectoryId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "ExchangeId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Extension",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "FloorId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "IP",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "category",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "deviceName",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "floorX",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "floorY",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "remarks",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "seatingCapacity",
                table: "Locations");
        }
    }
}
