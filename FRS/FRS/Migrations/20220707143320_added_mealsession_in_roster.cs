using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_mealsession_in_roster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MealSessionId",
                table: "OutletClassRosters",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutletClassRosters_MealSessionId",
                table: "OutletClassRosters",
                column: "MealSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutletClassRosters_MealSessions_MealSessionId",
                table: "OutletClassRosters",
                column: "MealSessionId",
                principalTable: "MealSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutletClassRosters_MealSessions_MealSessionId",
                table: "OutletClassRosters");

            migrationBuilder.DropIndex(
                name: "IX_OutletClassRosters_MealSessionId",
                table: "OutletClassRosters");

            migrationBuilder.DropColumn(
                name: "MealSessionId",
                table: "OutletClassRosters");
        }
    }
}
