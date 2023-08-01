using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_outletdishcycletype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleSets_CycleTypeId",
                table: "DishCycleScheduleSets",
                column: "CycleTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DishCycleScheduleSets_DishCycles_CycleTypeId",
                table: "DishCycleScheduleSets",
                column: "CycleTypeId",
                principalTable: "DishCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishCycleScheduleSets_DishCycles_CycleTypeId",
                table: "DishCycleScheduleSets");

            migrationBuilder.DropIndex(
                name: "IX_DishCycleScheduleSets_CycleTypeId",
                table: "DishCycleScheduleSets");
        }
    }
}
