using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class updated_menucycletable_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleScheduleMenus_User_CreatedBy",
                table: "MenuCycleScheduleMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleScheduleMenus_MenuCycleSchedulePeriods_MenuCycleSchedulePeriodId",
                table: "MenuCycleScheduleMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleScheduleMenus_Menus_MenuId",
                table: "MenuCycleScheduleMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleScheduleMenus_User_UpdatedBy",
                table: "MenuCycleScheduleMenus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuCycleScheduleMenus",
                table: "MenuCycleScheduleMenus");

            migrationBuilder.RenameTable(
                name: "MenuCycleScheduleMenus",
                newName: "MenuCycleSchedulePeriodMenus");

            migrationBuilder.RenameIndex(
                name: "IX_MenuCycleScheduleMenus_UpdatedBy",
                table: "MenuCycleSchedulePeriodMenus",
                newName: "IX_MenuCycleSchedulePeriodMenus_UpdatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_MenuCycleScheduleMenus_MenuId",
                table: "MenuCycleSchedulePeriodMenus",
                newName: "IX_MenuCycleSchedulePeriodMenus_MenuId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuCycleScheduleMenus_CreatedBy",
                table: "MenuCycleSchedulePeriodMenus",
                newName: "IX_MenuCycleSchedulePeriodMenus_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuCycleSchedulePeriodMenus",
                table: "MenuCycleSchedulePeriodMenus",
                columns: new[] { "MenuCycleSchedulePeriodId", "MenuId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleSchedulePeriodMenus_User_CreatedBy",
                table: "MenuCycleSchedulePeriodMenus",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleSchedulePeriodMenus_MenuCycleSchedulePeriods_MenuCycleSchedulePeriodId",
                table: "MenuCycleSchedulePeriodMenus",
                column: "MenuCycleSchedulePeriodId",
                principalTable: "MenuCycleSchedulePeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleSchedulePeriodMenus_Menus_MenuId",
                table: "MenuCycleSchedulePeriodMenus",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleSchedulePeriodMenus_User_UpdatedBy",
                table: "MenuCycleSchedulePeriodMenus",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleSchedulePeriodMenus_User_CreatedBy",
                table: "MenuCycleSchedulePeriodMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleSchedulePeriodMenus_MenuCycleSchedulePeriods_MenuCycleSchedulePeriodId",
                table: "MenuCycleSchedulePeriodMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleSchedulePeriodMenus_Menus_MenuId",
                table: "MenuCycleSchedulePeriodMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleSchedulePeriodMenus_User_UpdatedBy",
                table: "MenuCycleSchedulePeriodMenus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuCycleSchedulePeriodMenus",
                table: "MenuCycleSchedulePeriodMenus");

            migrationBuilder.RenameTable(
                name: "MenuCycleSchedulePeriodMenus",
                newName: "MenuCycleScheduleMenus");

            migrationBuilder.RenameIndex(
                name: "IX_MenuCycleSchedulePeriodMenus_UpdatedBy",
                table: "MenuCycleScheduleMenus",
                newName: "IX_MenuCycleScheduleMenus_UpdatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_MenuCycleSchedulePeriodMenus_MenuId",
                table: "MenuCycleScheduleMenus",
                newName: "IX_MenuCycleScheduleMenus_MenuId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuCycleSchedulePeriodMenus_CreatedBy",
                table: "MenuCycleScheduleMenus",
                newName: "IX_MenuCycleScheduleMenus_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuCycleScheduleMenus",
                table: "MenuCycleScheduleMenus",
                columns: new[] { "MenuCycleSchedulePeriodId", "MenuId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleScheduleMenus_User_CreatedBy",
                table: "MenuCycleScheduleMenus",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleScheduleMenus_MenuCycleSchedulePeriods_MenuCycleSchedulePeriodId",
                table: "MenuCycleScheduleMenus",
                column: "MenuCycleSchedulePeriodId",
                principalTable: "MenuCycleSchedulePeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleScheduleMenus_Menus_MenuId",
                table: "MenuCycleScheduleMenus",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleScheduleMenus_User_UpdatedBy",
                table: "MenuCycleScheduleMenus",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
