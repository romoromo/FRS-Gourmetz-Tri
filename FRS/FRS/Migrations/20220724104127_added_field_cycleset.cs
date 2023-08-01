using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_field_cycleset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishCycleScheduleSets_DishCycles_DishCycleId",
                table: "DishCycleScheduleSets");

            migrationBuilder.AlterColumn<int>(
                name: "DishCycleId",
                table: "DishCycleScheduleSets",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DishCycleScheduleSets_DishCycles_DishCycleId",
                table: "DishCycleScheduleSets",
                column: "DishCycleId",
                principalTable: "DishCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishCycleScheduleSets_DishCycles_DishCycleId",
                table: "DishCycleScheduleSets");

            migrationBuilder.AlterColumn<int>(
                name: "DishCycleId",
                table: "DishCycleScheduleSets",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_DishCycleScheduleSets_DishCycles_DishCycleId",
                table: "DishCycleScheduleSets",
                column: "DishCycleId",
                principalTable: "DishCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
