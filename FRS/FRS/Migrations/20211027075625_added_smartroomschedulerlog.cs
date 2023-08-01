using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_smartroomschedulerlog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlternateMeetingTitle",
                table: "Reservations",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsKiosk",
                table: "Reservations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSignage",
                table: "Reservations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SmartRoomSchedulerLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EventDateTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    NoRecordsAffected = table.Column<int>(nullable: false),
                    Details = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartRoomSchedulerLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SmartRoomSchedulerLogs");

            migrationBuilder.DropColumn(
                name: "AlternateMeetingTitle",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "IsKiosk",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "IsSignage",
                table: "Reservations");
        }
    }
}
