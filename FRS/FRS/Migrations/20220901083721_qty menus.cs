using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class qtymenus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "qty_menus",
                table: "TokenLabels",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "qty_menus",
                table: "TokenLabels");
        }
    }
}
