using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_mealtype_dishcycle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MealTypeId",
                table: "DishCycleScheduleSets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleSets_MealTypeId",
                table: "DishCycleScheduleSets",
                column: "MealTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DishCycleScheduleSets_MealTypes_MealTypeId",
                table: "DishCycleScheduleSets",
                column: "MealTypeId",
                principalTable: "MealTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishCycleScheduleSets_MealTypes_MealTypeId",
                table: "DishCycleScheduleSets");

            migrationBuilder.DropIndex(
                name: "IX_DishCycleScheduleSets_MealTypeId",
                table: "DishCycleScheduleSets");

            migrationBuilder.DropColumn(
                name: "MealTypeId",
                table: "DishCycleScheduleSets");
        }
    }
}
