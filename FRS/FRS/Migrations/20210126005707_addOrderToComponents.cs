using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addOrderToComponents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "order",
                table: "SignageCompilationComponents",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "order",
                table: "SignageCompilationComponents");
        }
    }
}
