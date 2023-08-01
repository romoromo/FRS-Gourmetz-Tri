using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_menuschedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleSchedules_Menus_MenuId",
                table: "MenuCycleSchedules");

            migrationBuilder.DropIndex(
                name: "IX_MenuCycleSchedules_MenuId",
                table: "MenuCycleSchedules");

            migrationBuilder.DropColumn(
                name: "MenuId",
                table: "MenuCycleSchedules");

            migrationBuilder.CreateTable(
                name: "MenuCycleScheduleMenus",
                columns: table => new
                {
                    MenuCycleScheduleId = table.Column<int>(nullable: false),
                    MenuId = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCycleScheduleMenus", x => new { x.MenuCycleScheduleId, x.MenuId });
                    table.ForeignKey(
                        name: "FK_MenuCycleScheduleMenus_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuCycleScheduleMenus_MenuCycleSchedules_MenuCycleScheduleId",
                        column: x => x.MenuCycleScheduleId,
                        principalTable: "MenuCycleSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuCycleScheduleMenus_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuCycleScheduleMenus_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleScheduleMenus_CreatedBy",
                table: "MenuCycleScheduleMenus",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleScheduleMenus_MenuId",
                table: "MenuCycleScheduleMenus",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleScheduleMenus_UpdatedBy",
                table: "MenuCycleScheduleMenus",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuCycleScheduleMenus");

            migrationBuilder.AddColumn<int>(
                name: "MenuId",
                table: "MenuCycleSchedules",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleSchedules_MenuId",
                table: "MenuCycleSchedules",
                column: "MenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleSchedules_Menus_MenuId",
                table: "MenuCycleSchedules",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
