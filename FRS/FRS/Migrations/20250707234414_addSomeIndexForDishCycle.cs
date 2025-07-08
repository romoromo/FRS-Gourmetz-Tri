using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class addSomeIndexForDishCycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleSets_CycleTypeSequence",
                table: "DishCycleScheduleSets",
                column: "CycleTypeSequence");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleSchedules_Day",
                table: "DishCycleSchedules",
                column: "Day");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleDetails_Sequence",
                table: "DishCycleScheduleDetails",
                column: "Sequence");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycles_EndDate",
                table: "DishCycles",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycles_NumOfDays",
                table: "DishCycles",
                column: "NumOfDays");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycles_NumOfSets",
                table: "DishCycles",
                column: "NumOfSets");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycles_StartDate",
                table: "DishCycles",
                column: "StartDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DishCycleScheduleSets_CycleTypeSequence",
                table: "DishCycleScheduleSets");

            migrationBuilder.DropIndex(
                name: "IX_DishCycleSchedules_Day",
                table: "DishCycleSchedules");

            migrationBuilder.DropIndex(
                name: "IX_DishCycleScheduleDetails_Sequence",
                table: "DishCycleScheduleDetails");

            migrationBuilder.DropIndex(
                name: "IX_DishCycles_EndDate",
                table: "DishCycles");

            migrationBuilder.DropIndex(
                name: "IX_DishCycles_NumOfDays",
                table: "DishCycles");

            migrationBuilder.DropIndex(
                name: "IX_DishCycles_NumOfSets",
                table: "DishCycles");

            migrationBuilder.DropIndex(
                name: "IX_DishCycles_StartDate",
                table: "DishCycles");
        }
    }
}
