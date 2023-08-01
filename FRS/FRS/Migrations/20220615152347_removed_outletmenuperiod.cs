using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class removed_outletmenuperiod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutletMenuCycleSchedulePeriodMenus_OutletMenuCycleSchedulePeriods_OutletMenuCycleSchedulePeriodId",
                table: "OutletMenuCycleSchedulePeriodMenus");

            migrationBuilder.DropTable(
                name: "OutletMenuCycleSchedulePeriods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutletMenuCycleSchedulePeriodMenus",
                table: "OutletMenuCycleSchedulePeriodMenus");

            migrationBuilder.DropIndex(
                name: "IX_OutletMenuCycleSchedulePeriodMenus_OutletMenuCycleSchedulePeriodId",
                table: "OutletMenuCycleSchedulePeriodMenus");

            migrationBuilder.DropColumn(
                name: "OutletMenuCycleSchedulePeriodId",
                table: "OutletMenuCycleSchedulePeriodMenus");

            migrationBuilder.AddColumn<int>(
                name: "OutletId",
                table: "OutletMenuCycleSchedulePeriodMenus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutletMenuCycleSchedulePeriodMenus",
                table: "OutletMenuCycleSchedulePeriodMenus",
                columns: new[] { "MenuCycleSchedulePeriodId", "MenuId", "OutletId" });

            migrationBuilder.CreateIndex(
                name: "IX_OutletMenuCycleSchedulePeriodMenus_OutletId",
                table: "OutletMenuCycleSchedulePeriodMenus",
                column: "OutletId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutletMenuCycleSchedulePeriodMenus_Outlets_OutletId",
                table: "OutletMenuCycleSchedulePeriodMenus",
                column: "OutletId",
                principalTable: "Outlets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutletMenuCycleSchedulePeriodMenus_Outlets_OutletId",
                table: "OutletMenuCycleSchedulePeriodMenus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutletMenuCycleSchedulePeriodMenus",
                table: "OutletMenuCycleSchedulePeriodMenus");

            migrationBuilder.DropIndex(
                name: "IX_OutletMenuCycleSchedulePeriodMenus_OutletId",
                table: "OutletMenuCycleSchedulePeriodMenus");

            migrationBuilder.DropColumn(
                name: "OutletId",
                table: "OutletMenuCycleSchedulePeriodMenus");

            migrationBuilder.AddColumn<int>(
                name: "OutletMenuCycleSchedulePeriodId",
                table: "OutletMenuCycleSchedulePeriodMenus",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutletMenuCycleSchedulePeriodMenus",
                table: "OutletMenuCycleSchedulePeriodMenus",
                columns: new[] { "MenuCycleSchedulePeriodId", "MenuId" });

            migrationBuilder.CreateTable(
                name: "OutletMenuCycleSchedulePeriods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    MealPeriodId = table.Column<int>(nullable: false),
                    MenuCycleScheduleId = table.Column<int>(nullable: false),
                    OutletId = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_OutletMenuCycleSchedulePeriodMenus_OutletMenuCycleSchedulePeriodId",
                table: "OutletMenuCycleSchedulePeriodMenus",
                column: "OutletMenuCycleSchedulePeriodId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_OutletMenuCycleSchedulePeriodMenus_OutletMenuCycleSchedulePeriods_OutletMenuCycleSchedulePeriodId",
                table: "OutletMenuCycleSchedulePeriodMenus",
                column: "OutletMenuCycleSchedulePeriodId",
                principalTable: "OutletMenuCycleSchedulePeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
