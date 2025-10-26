using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddMealSessionDetailInClassLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MealSessionDetailId",
                table: "ClassLevels",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevels_MealSessionDetailId",
                table: "ClassLevels",
                column: "MealSessionDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevels_MealSessionDetails_MealSessionDetailId",
                table: "ClassLevels",
                column: "MealSessionDetailId",
                principalTable: "MealSessionDetails",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevels_MealSessionDetails_MealSessionDetailId",
                table: "ClassLevels");

            migrationBuilder.DropIndex(
                name: "IX_ClassLevels_MealSessionDetailId",
                table: "ClassLevels");

            migrationBuilder.DropColumn(
                name: "MealSessionDetailId",
                table: "ClassLevels");
        }
    }
}
