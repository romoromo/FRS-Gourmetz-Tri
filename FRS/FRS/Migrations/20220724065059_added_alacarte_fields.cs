using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_alacarte_fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishCycles_DishTypes_DishTypeId",
                table: "DishCycles");

            migrationBuilder.AddColumn<int>(
                name: "DishCycleId",
                table: "DishCycleScheduleDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DishCycleId",
                table: "DishCycleScheduleDetailMenus",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DishTypeId",
                table: "DishCycles",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "CycleType",
                table: "DishCycles",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleDetails_DishCycleId",
                table: "DishCycleScheduleDetails",
                column: "DishCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleDetailMenus_DishCycleId",
                table: "DishCycleScheduleDetailMenus",
                column: "DishCycleId");

            migrationBuilder.AddForeignKey(
                name: "FK_DishCycles_DishTypes_DishTypeId",
                table: "DishCycles",
                column: "DishTypeId",
                principalTable: "DishTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DishCycleScheduleDetailMenus_DishCycles_DishCycleId",
                table: "DishCycleScheduleDetailMenus",
                column: "DishCycleId",
                principalTable: "DishCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DishCycleScheduleDetails_DishCycles_DishCycleId",
                table: "DishCycleScheduleDetails",
                column: "DishCycleId",
                principalTable: "DishCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishCycles_DishTypes_DishTypeId",
                table: "DishCycles");

            migrationBuilder.DropForeignKey(
                name: "FK_DishCycleScheduleDetailMenus_DishCycles_DishCycleId",
                table: "DishCycleScheduleDetailMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_DishCycleScheduleDetails_DishCycles_DishCycleId",
                table: "DishCycleScheduleDetails");

            migrationBuilder.DropIndex(
                name: "IX_DishCycleScheduleDetails_DishCycleId",
                table: "DishCycleScheduleDetails");

            migrationBuilder.DropIndex(
                name: "IX_DishCycleScheduleDetailMenus_DishCycleId",
                table: "DishCycleScheduleDetailMenus");

            migrationBuilder.DropColumn(
                name: "DishCycleId",
                table: "DishCycleScheduleDetails");

            migrationBuilder.DropColumn(
                name: "DishCycleId",
                table: "DishCycleScheduleDetailMenus");

            migrationBuilder.DropColumn(
                name: "CycleType",
                table: "DishCycles");

            migrationBuilder.AlterColumn<int>(
                name: "DishTypeId",
                table: "DishCycles",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DishCycles_DishTypes_DishTypeId",
                table: "DishCycles",
                column: "DishTypeId",
                principalTable: "DishTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
