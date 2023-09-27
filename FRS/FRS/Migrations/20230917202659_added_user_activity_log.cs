using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_user_activity_log : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActionName",
                table: "AuditLogs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupId",
                table: "AuditLogs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "AuditLogs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionName",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "AuditLogs");
        }
    }
}
