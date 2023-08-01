using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class rename_cycleset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishCycleScheduleSets_DishCycles_DishCycleTypeId",
                table: "DishCycleScheduleSets");

            migrationBuilder.DropIndex(
                name: "IX_DishCycleScheduleSets_DishCycleTypeId",
                table: "DishCycleScheduleSets");

            migrationBuilder.DropColumn(
                name: "DishCycleTypeId",
                table: "DishCycleScheduleSets");

            migrationBuilder.AddColumn<string>(
                name: "CycleType",
                table: "DishCycleScheduleSets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DishCycleId",
                table: "DishCycleScheduleSets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleSets_DishCycleId",
                table: "DishCycleScheduleSets",
                column: "DishCycleId");

            migrationBuilder.AddForeignKey(
                name: "FK_DishCycleScheduleSets_DishCycles_DishCycleId",
                table: "DishCycleScheduleSets",
                column: "DishCycleId",
                principalTable: "DishCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishCycleScheduleSets_DishCycles_DishCycleId",
                table: "DishCycleScheduleSets");

            migrationBuilder.DropIndex(
                name: "IX_DishCycleScheduleSets_DishCycleId",
                table: "DishCycleScheduleSets");

            migrationBuilder.DropColumn(
                name: "CycleType",
                table: "DishCycleScheduleSets");

            migrationBuilder.DropColumn(
                name: "DishCycleId",
                table: "DishCycleScheduleSets");

            migrationBuilder.AddColumn<int>(
                name: "DishCycleTypeId",
                table: "DishCycleScheduleSets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleSets_DishCycleTypeId",
                table: "DishCycleScheduleSets",
                column: "DishCycleTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DishCycleScheduleSets_DishCycles_DishCycleTypeId",
                table: "DishCycleScheduleSets",
                column: "DishCycleTypeId",
                principalTable: "DishCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
