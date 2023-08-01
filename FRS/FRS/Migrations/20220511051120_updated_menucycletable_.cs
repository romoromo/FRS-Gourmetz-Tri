using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class updated_menucycletable_ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleScheduleMenus_MenuCycleSchedulePeriod_MenuCycleSchedulePeriodId",
                table: "MenuCycleScheduleMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleSchedulePeriod_User_CreatedBy",
                table: "MenuCycleSchedulePeriod");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleSchedulePeriod_MealPeriods_MealPeriodId",
                table: "MenuCycleSchedulePeriod");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleSchedulePeriod_MenuCycleSchedules_MenuCycleScheduleId",
                table: "MenuCycleSchedulePeriod");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleSchedulePeriod_User_UpdatedBy",
                table: "MenuCycleSchedulePeriod");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuCycleSchedulePeriod",
                table: "MenuCycleSchedulePeriod");

            migrationBuilder.RenameTable(
                name: "MenuCycleSchedulePeriod",
                newName: "MenuCycleSchedulePeriods");

            migrationBuilder.RenameIndex(
                name: "IX_MenuCycleSchedulePeriod_UpdatedBy",
                table: "MenuCycleSchedulePeriods",
                newName: "IX_MenuCycleSchedulePeriods_UpdatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_MenuCycleSchedulePeriod_MenuCycleScheduleId",
                table: "MenuCycleSchedulePeriods",
                newName: "IX_MenuCycleSchedulePeriods_MenuCycleScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuCycleSchedulePeriod_MealPeriodId",
                table: "MenuCycleSchedulePeriods",
                newName: "IX_MenuCycleSchedulePeriods_MealPeriodId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuCycleSchedulePeriod_CreatedBy",
                table: "MenuCycleSchedulePeriods",
                newName: "IX_MenuCycleSchedulePeriods_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuCycleSchedulePeriods",
                table: "MenuCycleSchedulePeriods",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleScheduleMenus_MenuCycleSchedulePeriods_MenuCycleSchedulePeriodId",
                table: "MenuCycleScheduleMenus",
                column: "MenuCycleSchedulePeriodId",
                principalTable: "MenuCycleSchedulePeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleSchedulePeriods_User_CreatedBy",
                table: "MenuCycleSchedulePeriods",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleSchedulePeriods_MealPeriods_MealPeriodId",
                table: "MenuCycleSchedulePeriods",
                column: "MealPeriodId",
                principalTable: "MealPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleSchedulePeriods_MenuCycleSchedules_MenuCycleScheduleId",
                table: "MenuCycleSchedulePeriods",
                column: "MenuCycleScheduleId",
                principalTable: "MenuCycleSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleSchedulePeriods_User_UpdatedBy",
                table: "MenuCycleSchedulePeriods",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleScheduleMenus_MenuCycleSchedulePeriods_MenuCycleSchedulePeriodId",
                table: "MenuCycleScheduleMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleSchedulePeriods_User_CreatedBy",
                table: "MenuCycleSchedulePeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleSchedulePeriods_MealPeriods_MealPeriodId",
                table: "MenuCycleSchedulePeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleSchedulePeriods_MenuCycleSchedules_MenuCycleScheduleId",
                table: "MenuCycleSchedulePeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleSchedulePeriods_User_UpdatedBy",
                table: "MenuCycleSchedulePeriods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuCycleSchedulePeriods",
                table: "MenuCycleSchedulePeriods");

            migrationBuilder.RenameTable(
                name: "MenuCycleSchedulePeriods",
                newName: "MenuCycleSchedulePeriod");

            migrationBuilder.RenameIndex(
                name: "IX_MenuCycleSchedulePeriods_UpdatedBy",
                table: "MenuCycleSchedulePeriod",
                newName: "IX_MenuCycleSchedulePeriod_UpdatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_MenuCycleSchedulePeriods_MenuCycleScheduleId",
                table: "MenuCycleSchedulePeriod",
                newName: "IX_MenuCycleSchedulePeriod_MenuCycleScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuCycleSchedulePeriods_MealPeriodId",
                table: "MenuCycleSchedulePeriod",
                newName: "IX_MenuCycleSchedulePeriod_MealPeriodId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuCycleSchedulePeriods_CreatedBy",
                table: "MenuCycleSchedulePeriod",
                newName: "IX_MenuCycleSchedulePeriod_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuCycleSchedulePeriod",
                table: "MenuCycleSchedulePeriod",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleScheduleMenus_MenuCycleSchedulePeriod_MenuCycleSchedulePeriodId",
                table: "MenuCycleScheduleMenus",
                column: "MenuCycleSchedulePeriodId",
                principalTable: "MenuCycleSchedulePeriod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleSchedulePeriod_User_CreatedBy",
                table: "MenuCycleSchedulePeriod",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleSchedulePeriod_MealPeriods_MealPeriodId",
                table: "MenuCycleSchedulePeriod",
                column: "MealPeriodId",
                principalTable: "MealPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleSchedulePeriod_MenuCycleSchedules_MenuCycleScheduleId",
                table: "MenuCycleSchedulePeriod",
                column: "MenuCycleScheduleId",
                principalTable: "MenuCycleSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleSchedulePeriod_User_UpdatedBy",
                table: "MenuCycleSchedulePeriod",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
