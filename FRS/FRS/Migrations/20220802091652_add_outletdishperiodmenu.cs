using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class add_outletdishperiodmenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutletDishCyclePeriodMenu_User_CreatedBy",
                table: "OutletDishCyclePeriodMenu");

            migrationBuilder.DropForeignKey(
                name: "FK_OutletDishCyclePeriodMenu_DishCyclePeriods_DishCyclePeriodId",
                table: "OutletDishCyclePeriodMenu");

            migrationBuilder.DropForeignKey(
                name: "FK_OutletDishCyclePeriodMenu_DishCycleScheduleSets_DishCycleScheduleSetId",
                table: "OutletDishCyclePeriodMenu");

            migrationBuilder.DropForeignKey(
                name: "FK_OutletDishCyclePeriodMenu_Dishes_DishId",
                table: "OutletDishCyclePeriodMenu");

            migrationBuilder.DropForeignKey(
                name: "FK_OutletDishCyclePeriodMenu_Outlets_OutletId",
                table: "OutletDishCyclePeriodMenu");

            migrationBuilder.DropForeignKey(
                name: "FK_OutletDishCyclePeriodMenu_User_UpdatedBy",
                table: "OutletDishCyclePeriodMenu");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutletDishCyclePeriodMenu",
                table: "OutletDishCyclePeriodMenu");

            migrationBuilder.RenameTable(
                name: "OutletDishCyclePeriodMenu",
                newName: "OutletDishCyclePeriodMenus");

            migrationBuilder.RenameIndex(
                name: "IX_OutletDishCyclePeriodMenu_UpdatedBy",
                table: "OutletDishCyclePeriodMenus",
                newName: "IX_OutletDishCyclePeriodMenus_UpdatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_OutletDishCyclePeriodMenu_OutletId",
                table: "OutletDishCyclePeriodMenus",
                newName: "IX_OutletDishCyclePeriodMenus_OutletId");

            migrationBuilder.RenameIndex(
                name: "IX_OutletDishCyclePeriodMenu_DishId",
                table: "OutletDishCyclePeriodMenus",
                newName: "IX_OutletDishCyclePeriodMenus_DishId");

            migrationBuilder.RenameIndex(
                name: "IX_OutletDishCyclePeriodMenu_DishCycleScheduleSetId",
                table: "OutletDishCyclePeriodMenus",
                newName: "IX_OutletDishCyclePeriodMenus_DishCycleScheduleSetId");

            migrationBuilder.RenameIndex(
                name: "IX_OutletDishCyclePeriodMenu_CreatedBy",
                table: "OutletDishCyclePeriodMenus",
                newName: "IX_OutletDishCyclePeriodMenus_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutletDishCyclePeriodMenus",
                table: "OutletDishCyclePeriodMenus",
                columns: new[] { "DishCyclePeriodId", "DishId", "OutletId", "DishCycleScheduleSetId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OutletDishCyclePeriodMenus_User_CreatedBy",
                table: "OutletDishCyclePeriodMenus",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OutletDishCyclePeriodMenus_DishCyclePeriods_DishCyclePeriodId",
                table: "OutletDishCyclePeriodMenus",
                column: "DishCyclePeriodId",
                principalTable: "DishCyclePeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OutletDishCyclePeriodMenus_DishCycleScheduleSets_DishCycleScheduleSetId",
                table: "OutletDishCyclePeriodMenus",
                column: "DishCycleScheduleSetId",
                principalTable: "DishCycleScheduleSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OutletDishCyclePeriodMenus_Dishes_DishId",
                table: "OutletDishCyclePeriodMenus",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OutletDishCyclePeriodMenus_Outlets_OutletId",
                table: "OutletDishCyclePeriodMenus",
                column: "OutletId",
                principalTable: "Outlets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OutletDishCyclePeriodMenus_User_UpdatedBy",
                table: "OutletDishCyclePeriodMenus",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutletDishCyclePeriodMenus_User_CreatedBy",
                table: "OutletDishCyclePeriodMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_OutletDishCyclePeriodMenus_DishCyclePeriods_DishCyclePeriodId",
                table: "OutletDishCyclePeriodMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_OutletDishCyclePeriodMenus_DishCycleScheduleSets_DishCycleScheduleSetId",
                table: "OutletDishCyclePeriodMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_OutletDishCyclePeriodMenus_Dishes_DishId",
                table: "OutletDishCyclePeriodMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_OutletDishCyclePeriodMenus_Outlets_OutletId",
                table: "OutletDishCyclePeriodMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_OutletDishCyclePeriodMenus_User_UpdatedBy",
                table: "OutletDishCyclePeriodMenus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutletDishCyclePeriodMenus",
                table: "OutletDishCyclePeriodMenus");

            migrationBuilder.RenameTable(
                name: "OutletDishCyclePeriodMenus",
                newName: "OutletDishCyclePeriodMenu");

            migrationBuilder.RenameIndex(
                name: "IX_OutletDishCyclePeriodMenus_UpdatedBy",
                table: "OutletDishCyclePeriodMenu",
                newName: "IX_OutletDishCyclePeriodMenu_UpdatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_OutletDishCyclePeriodMenus_OutletId",
                table: "OutletDishCyclePeriodMenu",
                newName: "IX_OutletDishCyclePeriodMenu_OutletId");

            migrationBuilder.RenameIndex(
                name: "IX_OutletDishCyclePeriodMenus_DishId",
                table: "OutletDishCyclePeriodMenu",
                newName: "IX_OutletDishCyclePeriodMenu_DishId");

            migrationBuilder.RenameIndex(
                name: "IX_OutletDishCyclePeriodMenus_DishCycleScheduleSetId",
                table: "OutletDishCyclePeriodMenu",
                newName: "IX_OutletDishCyclePeriodMenu_DishCycleScheduleSetId");

            migrationBuilder.RenameIndex(
                name: "IX_OutletDishCyclePeriodMenus_CreatedBy",
                table: "OutletDishCyclePeriodMenu",
                newName: "IX_OutletDishCyclePeriodMenu_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutletDishCyclePeriodMenu",
                table: "OutletDishCyclePeriodMenu",
                columns: new[] { "DishCyclePeriodId", "DishId", "OutletId", "DishCycleScheduleSetId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OutletDishCyclePeriodMenu_User_CreatedBy",
                table: "OutletDishCyclePeriodMenu",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OutletDishCyclePeriodMenu_DishCyclePeriods_DishCyclePeriodId",
                table: "OutletDishCyclePeriodMenu",
                column: "DishCyclePeriodId",
                principalTable: "DishCyclePeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OutletDishCyclePeriodMenu_DishCycleScheduleSets_DishCycleScheduleSetId",
                table: "OutletDishCyclePeriodMenu",
                column: "DishCycleScheduleSetId",
                principalTable: "DishCycleScheduleSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OutletDishCyclePeriodMenu_Dishes_DishId",
                table: "OutletDishCyclePeriodMenu",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OutletDishCyclePeriodMenu_Outlets_OutletId",
                table: "OutletDishCyclePeriodMenu",
                column: "OutletId",
                principalTable: "Outlets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OutletDishCyclePeriodMenu_User_UpdatedBy",
                table: "OutletDishCyclePeriodMenu",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
