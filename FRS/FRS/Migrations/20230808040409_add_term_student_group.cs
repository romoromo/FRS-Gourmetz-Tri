using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class add_term_student_group : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Term",
                table: "StudentGroups",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Term",
                table: "StudentGroups");
        }
    }
}
