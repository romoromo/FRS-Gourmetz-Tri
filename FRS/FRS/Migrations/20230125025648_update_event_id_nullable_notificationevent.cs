using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class update_event_id_nullable_notificationevent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationEvents_EventId",
                table: "Notifications");

            migrationBuilder.AlterColumn<int>(
                name: "EventId",
                table: "Notifications",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationEvents_EventId",
                table: "Notifications",
                column: "EventId",
                principalTable: "NotificationEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationEvents_EventId",
                table: "Notifications");

            migrationBuilder.AlterColumn<int>(
                name: "EventId",
                table: "Notifications",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationEvents_EventId",
                table: "Notifications",
                column: "EventId",
                principalTable: "NotificationEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
