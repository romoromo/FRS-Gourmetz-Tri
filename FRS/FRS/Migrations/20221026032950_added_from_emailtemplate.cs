using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_from_emailtemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FromEmail",
                table: "EmailTemplates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromName",
                table: "EmailTemplates",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromEmail",
                table: "EmailTemplates");

            migrationBuilder.DropColumn(
                name: "FromName",
                table: "EmailTemplates");
        }
    }
}
