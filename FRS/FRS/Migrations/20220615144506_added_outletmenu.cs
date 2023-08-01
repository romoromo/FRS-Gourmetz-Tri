using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_outletmenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutletMenuCycleSchedulePeriods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    MenuCycleScheduleId = table.Column<int>(nullable: false),
                    MealPeriodId = table.Column<int>(nullable: false),
                    OutletId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletMenuCycleSchedulePeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutletMenuCycleSchedulePeriods_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletMenuCycleSchedulePeriods_MealPeriods_MealPeriodId",
                        column: x => x.MealPeriodId,
                        principalTable: "MealPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutletMenuCycleSchedulePeriods_MenuCycleSchedules_MenuCycleScheduleId",
                        column: x => x.MenuCycleScheduleId,
                        principalTable: "MenuCycleSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutletMenuCycleSchedulePeriods_Outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutletMenuCycleSchedulePeriods_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutletMenuCycleSchedulePeriodMenus",
                columns: table => new
                {
                    MenuCycleSchedulePeriodId = table.Column<int>(nullable: false),
                    MenuId = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    OutletMenuCycleSchedulePeriodId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletMenuCycleSchedulePeriodMenus", x => new { x.MenuCycleSchedulePeriodId, x.MenuId });
                    table.ForeignKey(
                        name: "FK_OutletMenuCycleSchedulePeriodMenus_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletMenuCycleSchedulePeriodMenus_MenuCycleSchedulePeriods_MenuCycleSchedulePeriodId",
                        column: x => x.MenuCycleSchedulePeriodId,
                        principalTable: "MenuCycleSchedulePeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutletMenuCycleSchedulePeriodMenus_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutletMenuCycleSchedulePeriodMenus_OutletMenuCycleSchedulePeriods_OutletMenuCycleSchedulePeriodId",
                        column: x => x.OutletMenuCycleSchedulePeriodId,
                        principalTable: "OutletMenuCycleSchedulePeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletMenuCycleSchedulePeriodMenus_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutletMenuCycleSchedulePeriodMenus_CreatedBy",
                table: "OutletMenuCycleSchedulePeriodMenus",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutletMenuCycleSchedulePeriodMenus_MenuId",
                table: "OutletMenuCycleSchedulePeriodMenus",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletMenuCycleSchedulePeriodMenus_OutletMenuCycleSchedulePeriodId",
                table: "OutletMenuCycleSchedulePeriodMenus",
                column: "OutletMenuCycleSchedulePeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletMenuCycleSchedulePeriodMenus_UpdatedBy",
                table: "OutletMenuCycleSchedulePeriodMenus",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutletMenuCycleSchedulePeriods_CreatedBy",
                table: "OutletMenuCycleSchedulePeriods",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutletMenuCycleSchedulePeriods_MealPeriodId",
                table: "OutletMenuCycleSchedulePeriods",
                column: "MealPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletMenuCycleSchedulePeriods_MenuCycleScheduleId",
                table: "OutletMenuCycleSchedulePeriods",
                column: "MenuCycleScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletMenuCycleSchedulePeriods_OutletId",
                table: "OutletMenuCycleSchedulePeriods",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletMenuCycleSchedulePeriods_UpdatedBy",
                table: "OutletMenuCycleSchedulePeriods",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutletMenuCycleSchedulePeriodMenus");

            migrationBuilder.DropTable(
                name: "OutletMenuCycleSchedulePeriods");
        }
    }
}
