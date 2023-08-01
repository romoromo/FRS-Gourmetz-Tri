using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_message_externallogin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "ExternalAppLoginLogs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "ExternalAppLoginLogs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "ExternalAppLoginLogs");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "ExternalAppLoginLogs");
        }
    }
}
