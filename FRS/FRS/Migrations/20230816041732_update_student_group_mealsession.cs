using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class update_student_group_mealsession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentGroups_MealSessions_MealSessionId",
                table: "StudentGroups");

            migrationBuilder.DropIndex(
                name: "IX_StudentGroups_MealSessionId",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "MealSessionId",
                table: "StudentGroups");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MealSessionId",
                table: "StudentGroups",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroups_MealSessionId",
                table: "StudentGroups",
                column: "MealSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentGroups_MealSessions_MealSessionId",
                table: "StudentGroups",
                column: "MealSessionId",
                principalTable: "MealSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
