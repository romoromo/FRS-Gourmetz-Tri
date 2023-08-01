using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_fk_reservation_smartroom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SmartRoomScheduleId",
                table: "Reservations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SmartRoomResourceId",
                table: "Locations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_SmartRoomScheduleId",
                table: "Reservations",
                column: "SmartRoomScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_SmartRoomResourceId",
                table: "Locations",
                column: "SmartRoomResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_SmartRoomResources_SmartRoomResourceId",
                table: "Locations",
                column: "SmartRoomResourceId",
                principalTable: "SmartRoomResources",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_SmartRoomSchedules_SmartRoomScheduleId",
                table: "Reservations",
                column: "SmartRoomScheduleId",
                principalTable: "SmartRoomSchedules",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_SmartRoomResources_SmartRoomResourceId",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_SmartRoomSchedules_SmartRoomScheduleId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_SmartRoomScheduleId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Locations_SmartRoomResourceId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "SmartRoomScheduleId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "SmartRoomResourceId",
                table: "Locations");
        }
    }
}
