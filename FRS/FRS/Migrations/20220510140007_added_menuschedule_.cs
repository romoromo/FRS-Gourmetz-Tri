using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_menuschedule_ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleScheduleMenus_MenuCycleSchedules_MenuCycleScheduleId",
                table: "MenuCycleScheduleMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleSchedules_MealPeriods_MealPeriodId",
                table: "MenuCycleSchedules");

            migrationBuilder.DropIndex(
                name: "IX_MenuCycleSchedules_MealPeriodId",
                table: "MenuCycleSchedules");

            migrationBuilder.DropColumn(
                name: "MealPeriodId",
                table: "MenuCycleSchedules");

            migrationBuilder.RenameColumn(
                name: "MenuCycleScheduleId",
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

            migrationBuilder.CreateTable(
                name: "MenuCycleSchedulePeriod",
                columns: table => new
                {
                    MenuCycleScheduleId = table.Column<int>(nullable: false),
                    MealPeriodId = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCycleSchedulePeriod", x => new { x.MenuCycleScheduleId, x.MealPeriodId });
                    table.ForeignKey(
                        name: "FK_MenuCycleSchedulePeriod_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuCycleSchedulePeriod_MealPeriods_MealPeriodId",
                        column: x => x.MealPeriodId,
                        principalTable: "MealPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuCycleSchedulePeriod_MenuCycleSchedules_MenuCycleScheduleId",
                        column: x => x.MenuCycleScheduleId,
                        principalTable: "MenuCycleSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuCycleSchedulePeriod_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleScheduleMenus_MenuCycleSchedulePeriodMenuCycleScheduleId_MenuCycleSchedulePeriodMealPeriodId",
                table: "MenuCycleScheduleMenus",
                columns: new[] { "MenuCycleSchedulePeriodMenuCycleScheduleId", "MenuCycleSchedulePeriodMealPeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleSchedulePeriod_CreatedBy",
                table: "MenuCycleSchedulePeriod",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleSchedulePeriod_MealPeriodId",
                table: "MenuCycleSchedulePeriod",
                column: "MealPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleSchedulePeriod_UpdatedBy",
                table: "MenuCycleSchedulePeriod",
                column: "UpdatedBy");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleScheduleMenus_MealPeriods_MealPeriodId",
                table: "MenuCycleScheduleMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleScheduleMenus_MenuCycleSchedulePeriod_MenuCycleSchedulePeriodMenuCycleScheduleId_MenuCycleSchedulePeriodMealPeriodId",
                table: "MenuCycleScheduleMenus");

            migrationBuilder.DropTable(
                name: "MenuCycleSchedulePeriod");

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
                newName: "MenuCycleScheduleId");

            migrationBuilder.AddColumn<int>(
                name: "MealPeriodId",
                table: "MenuCycleSchedules",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleSchedules_MealPeriodId",
                table: "MenuCycleSchedules",
                column: "MealPeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleScheduleMenus_MenuCycleSchedules_MenuCycleScheduleId",
                table: "MenuCycleScheduleMenus",
                column: "MenuCycleScheduleId",
                principalTable: "MenuCycleSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleSchedules_MealPeriods_MealPeriodId",
                table: "MenuCycleSchedules",
                column: "MealPeriodId",
                principalTable: "MealPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
