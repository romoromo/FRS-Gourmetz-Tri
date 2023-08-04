using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_price_student_group : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Price",
                table: "StudentGroups",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "StudentGroups");
        }
    }
}
