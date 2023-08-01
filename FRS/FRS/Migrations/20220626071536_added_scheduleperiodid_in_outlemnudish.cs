using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_scheduleperiodid_in_outlemnudish : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutletMenuDishes_OutletMenuCycleSchedulePeriodMenus_OutletMenuCycleSchedulePeriodMenuMenuCycleSchedulePeriodId_OutletMenuCyc~",
                table: "OutletMenuDishes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutletMenuDishes",
                table: "OutletMenuDishes");

            migrationBuilder.DropIndex(
                name: "IX_OutletMenuDishes_OutletMenuCycleSchedulePeriodMenuMenuCycleSchedulePeriodId_OutletMenuCycleSchedulePeriodMenuMenuId_OutletMe~",
                table: "OutletMenuDishes");

            migrationBuilder.DropColumn(
                name: "OutletMenuCycleSchedulePeriodMenuMenuCycleSchedulePeriodId",
                table: "OutletMenuDishes");

            migrationBuilder.DropColumn(
                name: "OutletMenuCycleSchedulePeriodMenuMenuId",
                table: "OutletMenuDishes");

            migrationBuilder.DropColumn(
                name: "OutletMenuCycleSchedulePeriodMenuOutletId",
                table: "OutletMenuDishes");

            migrationBuilder.AddColumn<int>(
                name: "MenuCycleSchedulePeriodId",
                table: "OutletMenuDishes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutletMenuDishes",
                table: "OutletMenuDishes",
                columns: new[] { "MenuCycleSchedulePeriodId", "DishId", "MenuId", "MealTypeId", "OutletId" });

            migrationBuilder.CreateIndex(
                name: "IX_OutletMenuDishes_DishId",
                table: "OutletMenuDishes",
                column: "DishId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutletMenuDishes_MenuCycleSchedulePeriods_MenuCycleSchedulePeriodId",
                table: "OutletMenuDishes",
                column: "MenuCycleSchedulePeriodId",
                principalTable: "MenuCycleSchedulePeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutletMenuDishes_MenuCycleSchedulePeriods_MenuCycleSchedulePeriodId",
                table: "OutletMenuDishes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutletMenuDishes",
                table: "OutletMenuDishes");

            migrationBuilder.DropIndex(
                name: "IX_OutletMenuDishes_DishId",
                table: "OutletMenuDishes");

            migrationBuilder.DropColumn(
                name: "MenuCycleSchedulePeriodId",
                table: "OutletMenuDishes");

            migrationBuilder.AddColumn<int>(
                name: "OutletMenuCycleSchedulePeriodMenuMenuCycleSchedulePeriodId",
                table: "OutletMenuDishes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OutletMenuCycleSchedulePeriodMenuMenuId",
                table: "OutletMenuDishes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OutletMenuCycleSchedulePeriodMenuOutletId",
                table: "OutletMenuDishes",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutletMenuDishes",
                table: "OutletMenuDishes",
                columns: new[] { "DishId", "MenuId", "MealTypeId", "OutletId" });

            migrationBuilder.CreateIndex(
                name: "IX_OutletMenuDishes_OutletMenuCycleSchedulePeriodMenuMenuCycleSchedulePeriodId_OutletMenuCycleSchedulePeriodMenuMenuId_OutletMe~",
                table: "OutletMenuDishes",
                columns: new[] { "OutletMenuCycleSchedulePeriodMenuMenuCycleSchedulePeriodId", "OutletMenuCycleSchedulePeriodMenuMenuId", "OutletMenuCycleSchedulePeriodMenuOutletId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OutletMenuDishes_OutletMenuCycleSchedulePeriodMenus_OutletMenuCycleSchedulePeriodMenuMenuCycleSchedulePeriodId_OutletMenuCyc~",
                table: "OutletMenuDishes",
                columns: new[] { "OutletMenuCycleSchedulePeriodMenuMenuCycleSchedulePeriodId", "OutletMenuCycleSchedulePeriodMenuMenuId", "OutletMenuCycleSchedulePeriodMenuOutletId" },
                principalTable: "OutletMenuCycleSchedulePeriodMenus",
                principalColumns: new[] { "MenuCycleSchedulePeriodId", "MenuId", "OutletId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
