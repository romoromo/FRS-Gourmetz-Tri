using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_caterer_mealsession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CatererId",
                table: "MealSessions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealSessions_CatererId",
                table: "MealSessions",
                column: "CatererId");

            migrationBuilder.AddForeignKey(
                name: "FK_MealSessions_CatererInfos_CatererId",
                table: "MealSessions",
                column: "CatererId",
                principalTable: "CatererInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealSessions_CatererInfos_CatererId",
                table: "MealSessions");

            migrationBuilder.DropIndex(
                name: "IX_MealSessions_CatererId",
                table: "MealSessions");

            migrationBuilder.DropColumn(
                name: "CatererId",
                table: "MealSessions");
        }
    }
}
