using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_new_sg_fields_sequence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "StudentGroups",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "StudentGroups");
        }
    }
}
