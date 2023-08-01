using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class remove_type_template_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemplateType",
                table: "PIBTemplates");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TemplateType",
                table: "PIBTemplates",
                nullable: true);
        }
    }
}
