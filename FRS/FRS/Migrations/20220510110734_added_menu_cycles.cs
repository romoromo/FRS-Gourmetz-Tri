using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_menu_cycles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuCycles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    InstitutionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCycles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuCycles_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuCycles_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuCycles_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MenuCycleSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    MenuCycleId = table.Column<int>(nullable: false),
                    MenuId = table.Column<int>(nullable: true),
                    MealPeriodId = table.Column<int>(nullable: false),
                    Day = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCycleSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuCycleSchedules_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuCycleSchedules_MealPeriods_MealPeriodId",
                        column: x => x.MealPeriodId,
                        principalTable: "MealPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuCycleSchedules_MenuCycles_MenuCycleId",
                        column: x => x.MenuCycleId,
                        principalTable: "MenuCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuCycleSchedules_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuCycleSchedules_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycles_CreatedBy",
                table: "MenuCycles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycles_InstitutionId",
                table: "MenuCycles",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycles_UpdatedBy",
                table: "MenuCycles",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleSchedules_CreatedBy",
                table: "MenuCycleSchedules",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleSchedules_MealPeriodId",
                table: "MenuCycleSchedules",
                column: "MealPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleSchedules_MenuCycleId",
                table: "MenuCycleSchedules",
                column: "MenuCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleSchedules_MenuId",
                table: "MenuCycleSchedules",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleSchedules_UpdatedBy",
                table: "MenuCycleSchedules",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuCycleSchedules");

            migrationBuilder.DropTable(
                name: "MenuCycles");
        }
    }
}
