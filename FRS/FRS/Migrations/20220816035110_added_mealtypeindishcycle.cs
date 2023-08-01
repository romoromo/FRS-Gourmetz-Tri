using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_mealtypeindishcycle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MealTypeId",
                table: "DishCycles",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DishCycles_MealTypeId",
                table: "DishCycles",
                column: "MealTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DishCycles_MealTypes_MealTypeId",
                table: "DishCycles",
                column: "MealTypeId",
                principalTable: "MealTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishCycles_MealTypes_MealTypeId",
                table: "DishCycles");

            migrationBuilder.DropIndex(
                name: "IX_DishCycles_MealTypeId",
                table: "DishCycles");

            migrationBuilder.DropColumn(
                name: "MealTypeId",
                table: "DishCycles");
        }
    }
}
