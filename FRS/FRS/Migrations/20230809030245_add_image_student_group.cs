using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class add_image_student_group : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "StudentGroups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "StudentGroups",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "StudentGroups");
        }
    }
}
