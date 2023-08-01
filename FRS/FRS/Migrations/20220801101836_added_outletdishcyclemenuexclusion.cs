using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_outletdishcyclemenuexclusion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutletDishCyclePeriodMenu",
                columns: table => new
                {
                    DishCyclePeriodId = table.Column<int>(nullable: false),
                    MenuId = table.Column<int>(nullable: false),
                    OutletId = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletDishCyclePeriodMenu", x => new { x.DishCyclePeriodId, x.MenuId, x.OutletId });
                    table.ForeignKey(
                        name: "FK_OutletDishCyclePeriodMenu_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletDishCyclePeriodMenu_DishCyclePeriods_DishCyclePeriodId",
                        column: x => x.DishCyclePeriodId,
                        principalTable: "DishCyclePeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutletDishCyclePeriodMenu_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutletDishCyclePeriodMenu_Outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutletDishCyclePeriodMenu_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutletDishCyclePeriodMenu_CreatedBy",
                table: "OutletDishCyclePeriodMenu",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutletDishCyclePeriodMenu_MenuId",
                table: "OutletDishCyclePeriodMenu",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletDishCyclePeriodMenu_OutletId",
                table: "OutletDishCyclePeriodMenu",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletDishCyclePeriodMenu_UpdatedBy",
                table: "OutletDishCyclePeriodMenu",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutletDishCyclePeriodMenu");
        }
    }
}
