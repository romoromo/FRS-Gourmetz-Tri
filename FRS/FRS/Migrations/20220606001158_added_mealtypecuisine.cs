using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_mealtypecuisine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CuisineId",
                table: "MealTypes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealTypes_CuisineId",
                table: "MealTypes",
                column: "CuisineId");

            migrationBuilder.AddForeignKey(
                name: "FK_MealTypes_Cuisines_CuisineId",
                table: "MealTypes",
                column: "CuisineId",
                principalTable: "Cuisines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealTypes_Cuisines_CuisineId",
                table: "MealTypes");

            migrationBuilder.DropIndex(
                name: "IX_MealTypes_CuisineId",
                table: "MealTypes");

            migrationBuilder.DropColumn(
                name: "CuisineId",
                table: "MealTypes");
        }
    }
}
