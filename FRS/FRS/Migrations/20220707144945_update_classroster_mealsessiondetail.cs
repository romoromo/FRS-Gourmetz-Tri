using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class update_classroster_mealsessiondetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutletClassRosterSchedulePeriods_MealSessions_MealSessionId",
                table: "OutletClassRosterSchedulePeriods");

            migrationBuilder.RenameColumn(
                name: "MealSessionId",
                table: "OutletClassRosterSchedulePeriods",
                newName: "MealSessionDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_OutletClassRosterSchedulePeriods_MealSessionId",
                table: "OutletClassRosterSchedulePeriods",
                newName: "IX_OutletClassRosterSchedulePeriods_MealSessionDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutletClassRosterSchedulePeriods_MealSessionDetails_MealSessionDetailId",
                table: "OutletClassRosterSchedulePeriods",
                column: "MealSessionDetailId",
                principalTable: "MealSessionDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutletClassRosterSchedulePeriods_MealSessionDetails_MealSessionDetailId",
                table: "OutletClassRosterSchedulePeriods");

            migrationBuilder.RenameColumn(
                name: "MealSessionDetailId",
                table: "OutletClassRosterSchedulePeriods",
                newName: "MealSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_OutletClassRosterSchedulePeriods_MealSessionDetailId",
                table: "OutletClassRosterSchedulePeriods",
                newName: "IX_OutletClassRosterSchedulePeriods_MealSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutletClassRosterSchedulePeriods_MealSessions_MealSessionId",
                table: "OutletClassRosterSchedulePeriods",
                column: "MealSessionId",
                principalTable: "MealSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
