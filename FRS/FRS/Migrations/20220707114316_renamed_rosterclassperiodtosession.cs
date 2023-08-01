using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class renamed_rosterclassperiodtosession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutletClassRosterSchedulePeriods_MealPeriods_MealPeriodId",
                table: "OutletClassRosterSchedulePeriods");

            migrationBuilder.RenameColumn(
                name: "MealPeriodId",
                table: "OutletClassRosterSchedulePeriods",
                newName: "MealSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_OutletClassRosterSchedulePeriods_MealPeriodId",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutletClassRosterSchedulePeriods_MealSessions_MealSessionId",
                table: "OutletClassRosterSchedulePeriods");

            migrationBuilder.RenameColumn(
                name: "MealSessionId",
                table: "OutletClassRosterSchedulePeriods",
                newName: "MealPeriodId");

            migrationBuilder.RenameIndex(
                name: "IX_OutletClassRosterSchedulePeriods_MealSessionId",
                table: "OutletClassRosterSchedulePeriods",
                newName: "IX_OutletClassRosterSchedulePeriods_MealPeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutletClassRosterSchedulePeriods_MealPeriods_MealPeriodId",
                table: "OutletClassRosterSchedulePeriods",
                column: "MealPeriodId",
                principalTable: "MealPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
