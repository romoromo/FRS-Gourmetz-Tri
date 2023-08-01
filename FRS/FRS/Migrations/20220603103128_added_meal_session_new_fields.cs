using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_meal_session_new_fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "OverheadInterval",
                table: "MealSessionDetails",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "RouteInterval",
                table: "MealSessionDetails",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OverheadInterval",
                table: "MealSessionDetails");

            migrationBuilder.DropColumn(
                name: "RouteInterval",
                table: "MealSessionDetails");
        }
    }
}
