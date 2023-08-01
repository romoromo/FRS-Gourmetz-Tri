using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class updated_menucycletable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleScheduleMenus_MealPeriods_MealPeriodId",
                table: "MenuCycleScheduleMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleScheduleMenus_MenuCycleSchedulePeriod_MenuCycleSchedulePeriodMenuCycleScheduleId_MenuCycleSchedulePeriodMealPeriodId",
                table: "MenuCycleScheduleMenus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuCycleSchedulePeriod",
                table: "MenuCycleSchedulePeriod");

            migrationBuilder.DropIndex(
                name: "IX_MenuCycleScheduleMenus_MenuCycleSchedulePeriodMenuCycleScheduleId_MenuCycleSchedulePeriodMealPeriodId",
                table: "MenuCycleScheduleMenus");

            migrationBuilder.DropColumn(
                name: "MenuCycleSchedulePeriodMealPeriodId",
                table: "MenuCycleScheduleMenus");

            migrationBuilder.DropColumn(
                name: "MenuCycleSchedulePeriodMenuCycleScheduleId",
                table: "MenuCycleScheduleMenus");

            migrationBuilder.RenameColumn(
                name: "MealPeriodId",
                table: "MenuCycleScheduleMenus",
                newName: "MenuCycleSchedulePeriodId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MenuCycleSchedulePeriod",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuCycleSchedulePeriod",
                table: "MenuCycleSchedulePeriod",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleSchedulePeriod_MenuCycleScheduleId",
                table: "MenuCycleSchedulePeriod",
                column: "MenuCycleScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleScheduleMenus_MenuCycleSchedulePeriod_MenuCycleSchedulePeriodId",
                table: "MenuCycleScheduleMenus",
                column: "MenuCycleSchedulePeriodId",
                principalTable: "MenuCycleSchedulePeriod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleScheduleMenus_MenuCycleSchedulePeriod_MenuCycleSchedulePeriodId",
                table: "MenuCycleScheduleMenus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuCycleSchedulePeriod",
                table: "MenuCycleSchedulePeriod");

            migrationBuilder.DropIndex(
                name: "IX_MenuCycleSchedulePeriod_MenuCycleScheduleId",
                table: "MenuCycleSchedulePeriod");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MenuCycleSchedulePeriod");

            migrationBuilder.RenameColumn(
                name: "MenuCycleSchedulePeriodId",
                table: "MenuCycleScheduleMenus",
                newName: "MealPeriodId");

            migrationBuilder.AddColumn<int>(
                name: "MenuCycleSchedulePeriodMealPeriodId",
                table: "MenuCycleScheduleMenus",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MenuCycleSchedulePeriodMenuCycleScheduleId",
                table: "MenuCycleScheduleMenus",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuCycleSchedulePeriod",
                table: "MenuCycleSchedulePeriod",
                columns: new[] { "MenuCycleScheduleId", "MealPeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleScheduleMenus_MenuCycleSchedulePeriodMenuCycleScheduleId_MenuCycleSchedulePeriodMealPeriodId",
                table: "MenuCycleScheduleMenus",
                columns: new[] { "MenuCycleSchedulePeriodMenuCycleScheduleId", "MenuCycleSchedulePeriodMealPeriodId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleScheduleMenus_MealPeriods_MealPeriodId",
                table: "MenuCycleScheduleMenus",
                column: "MealPeriodId",
                principalTable: "MealPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleScheduleMenus_MenuCycleSchedulePeriod_MenuCycleSchedulePeriodMenuCycleScheduleId_MenuCycleSchedulePeriodMealPeriodId",
                table: "MenuCycleScheduleMenus",
                columns: new[] { "MenuCycleSchedulePeriodMenuCycleScheduleId", "MenuCycleSchedulePeriodMealPeriodId" },
                principalTable: "MenuCycleSchedulePeriod",
                principalColumns: new[] { "MenuCycleScheduleId", "MealPeriodId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
