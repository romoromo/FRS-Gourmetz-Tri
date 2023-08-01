using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class outletblockeddate_dishcycle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuCycleId",
                table: "OutletDishBlockedDates");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MenuCycleId",
                table: "OutletDishBlockedDates",
                nullable: true);
        }
    }
}
