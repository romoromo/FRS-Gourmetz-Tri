using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_mealtype_menudish : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuDishes",
                table: "MenuDishes");

            migrationBuilder.AddColumn<int>(
                name: "MealTypeId",
                table: "MenuDishes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuDishes",
                table: "MenuDishes",
                columns: new[] { "DishId", "MenuId", "MealTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_MenuDishes_MealTypeId",
                table: "MenuDishes",
                column: "MealTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuDishes_MealTypes_MealTypeId",
                table: "MenuDishes",
                column: "MealTypeId",
                principalTable: "MealTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuDishes_MealTypes_MealTypeId",
                table: "MenuDishes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuDishes",
                table: "MenuDishes");

            migrationBuilder.DropIndex(
                name: "IX_MenuDishes_MealTypeId",
                table: "MenuDishes");

            migrationBuilder.DropColumn(
                name: "MealTypeId",
                table: "MenuDishes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuDishes",
                table: "MenuDishes",
                columns: new[] { "DishId", "MenuId" });
        }
    }
}
