using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnMealSessionInClassLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MealSessionId",
                table: "ClassLevels",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevels_MealSessionId",
                table: "ClassLevels",
                column: "MealSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevels_MealSessions_MealSessionId",
                table: "ClassLevels",
                column: "MealSessionId",
                principalTable: "MealSessions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevels_MealSessions_MealSessionId",
                table: "ClassLevels");

            migrationBuilder.DropIndex(
                name: "IX_ClassLevels_MealSessionId",
                table: "ClassLevels");

            migrationBuilder.DropColumn(
                name: "MealSessionId",
                table: "ClassLevels");
        }
    }
}
