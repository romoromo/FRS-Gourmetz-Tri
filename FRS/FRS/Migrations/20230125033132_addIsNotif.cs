using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addIsNotif : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isNotifAbandonCart",
                table: "Students",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isNotifCancellationRequestStatus",
                table: "Students",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isNotifNoCardSetup",
                table: "Students",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isNotifNoOrderMadeForNextWeek",
                table: "Students",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isNotifAbandonCart",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "isNotifCancellationRequestStatus",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "isNotifNoCardSetup",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "isNotifNoOrderMadeForNextWeek",
                table: "Students");
        }
    }
}
