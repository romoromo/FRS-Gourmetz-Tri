using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_fk_reservation_smartroom_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "SmartRoomSchedules",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "SmartRoomResources",
                newName: "Id");

            migrationBuilder.AddColumn<int>(
                name: "SmartRoomScheduleID",
                table: "SmartRoomSchedules",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SmartRoomResourceID",
                table: "SmartRoomResources",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SmartRoomScheduleID",
                table: "SmartRoomSchedules");

            migrationBuilder.DropColumn(
                name: "SmartRoomResourceID",
                table: "SmartRoomResources");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SmartRoomSchedules",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SmartRoomResources",
                newName: "ID");
        }
    }
}
