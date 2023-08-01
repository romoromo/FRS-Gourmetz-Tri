using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class update_outletdishperiodmenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutletDishCyclePeriodMenu_Menus_MenuId",
                table: "OutletDishCyclePeriodMenu");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutletDishCyclePeriodMenu",
                table: "OutletDishCyclePeriodMenu");

            migrationBuilder.RenameColumn(
                name: "MenuId",
                table: "OutletDishCyclePeriodMenu",
                newName: "DishCycleScheduleSetId");

            migrationBuilder.RenameIndex(
                name: "IX_OutletDishCyclePeriodMenu_MenuId",
                table: "OutletDishCyclePeriodMenu",
                newName: "IX_OutletDishCyclePeriodMenu_DishCycleScheduleSetId");

            migrationBuilder.AddColumn<int>(
                name: "DishId",
                table: "OutletDishCyclePeriodMenu",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutletDishCyclePeriodMenu",
                table: "OutletDishCyclePeriodMenu",
                columns: new[] { "DishCyclePeriodId", "DishId", "OutletId", "DishCycleScheduleSetId" });

            migrationBuilder.CreateIndex(
                name: "IX_OutletDishCyclePeriodMenu_DishId",
                table: "OutletDishCyclePeriodMenu",
                column: "DishId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutletDishCyclePeriodMenu_DishCycleScheduleSets_DishCycleScheduleSetId",
                table: "OutletDishCyclePeriodMenu",
                column: "DishCycleScheduleSetId",
                principalTable: "DishCycleScheduleSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OutletDishCyclePeriodMenu_Dishes_DishId",
                table: "OutletDishCyclePeriodMenu",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutletDishCyclePeriodMenu_DishCycleScheduleSets_DishCycleScheduleSetId",
                table: "OutletDishCyclePeriodMenu");

            migrationBuilder.DropForeignKey(
                name: "FK_OutletDishCyclePeriodMenu_Dishes_DishId",
                table: "OutletDishCyclePeriodMenu");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutletDishCyclePeriodMenu",
                table: "OutletDishCyclePeriodMenu");

            migrationBuilder.DropIndex(
                name: "IX_OutletDishCyclePeriodMenu_DishId",
                table: "OutletDishCyclePeriodMenu");

            migrationBuilder.DropColumn(
                name: "DishId",
                table: "OutletDishCyclePeriodMenu");

            migrationBuilder.RenameColumn(
                name: "DishCycleScheduleSetId",
                table: "OutletDishCyclePeriodMenu",
                newName: "MenuId");

            migrationBuilder.RenameIndex(
                name: "IX_OutletDishCyclePeriodMenu_DishCycleScheduleSetId",
                table: "OutletDishCyclePeriodMenu",
                newName: "IX_OutletDishCyclePeriodMenu_MenuId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutletDishCyclePeriodMenu",
                table: "OutletDishCyclePeriodMenu",
                columns: new[] { "DishCyclePeriodId", "MenuId", "OutletId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OutletDishCyclePeriodMenu_Menus_MenuId",
                table: "OutletDishCyclePeriodMenu",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
