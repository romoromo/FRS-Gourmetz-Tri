using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class dishtokenorderqty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Qty",
                table: "TokenOrderDishes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Qty",
                table: "TokenOrderDishes");
        }
    }
}
