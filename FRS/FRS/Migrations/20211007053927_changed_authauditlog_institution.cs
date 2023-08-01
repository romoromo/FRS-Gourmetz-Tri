using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class changed_authauditlog_institution : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "AuthenticationLogs");

            migrationBuilder.AddColumn<string>(
                name: "InstitutionCode",
                table: "AuthenticationLogs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstitutionCode",
                table: "AuthenticationLogs");

            migrationBuilder.AddColumn<int>(
                name: "InstitutionId",
                table: "AuthenticationLogs",
                nullable: true);
        }
    }
}
