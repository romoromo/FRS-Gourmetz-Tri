using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_outlet_menudish : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutletMenuDishes",
                columns: table => new
                {
                    MenuId = table.Column<int>(nullable: false),
                    DishId = table.Column<int>(nullable: false),
                    MealTypeId = table.Column<int>(nullable: false),
                    OutletId = table.Column<int>(nullable: false),
                    OutletMenuCycleSchedulePeriodMenuMenuCycleSchedulePeriodId = table.Column<int>(nullable: true),
                    OutletMenuCycleSchedulePeriodMenuMenuId = table.Column<int>(nullable: true),
                    OutletMenuCycleSchedulePeriodMenuOutletId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletMenuDishes", x => new { x.DishId, x.MenuId, x.MealTypeId, x.OutletId });
                    table.ForeignKey(
                        name: "FK_OutletMenuDishes_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutletMenuDishes_MealTypes_MealTypeId",
                        column: x => x.MealTypeId,
                        principalTable: "MealTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutletMenuDishes_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutletMenuDishes_Outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutletMenuDishes_OutletMenuCycleSchedulePeriodMenus_OutletMenuCycleSchedulePeriodMenuMenuCycleSchedulePeriodId_OutletMenuCyc~",
                        columns: x => new { x.OutletMenuCycleSchedulePeriodMenuMenuCycleSchedulePeriodId, x.OutletMenuCycleSchedulePeriodMenuMenuId, x.OutletMenuCycleSchedulePeriodMenuOutletId },
                        principalTable: "OutletMenuCycleSchedulePeriodMenus",
                        principalColumns: new[] { "MenuCycleSchedulePeriodId", "MenuId", "OutletId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutletMenuDishes_MealTypeId",
                table: "OutletMenuDishes",
                column: "MealTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletMenuDishes_MenuId",
                table: "OutletMenuDishes",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletMenuDishes_OutletId",
                table: "OutletMenuDishes",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletMenuDishes_OutletMenuCycleSchedulePeriodMenuMenuCycleSchedulePeriodId_OutletMenuCycleSchedulePeriodMenuMenuId_OutletMe~",
                table: "OutletMenuDishes",
                columns: new[] { "OutletMenuCycleSchedulePeriodMenuMenuCycleSchedulePeriodId", "OutletMenuCycleSchedulePeriodMenuMenuId", "OutletMenuCycleSchedulePeriodMenuOutletId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutletMenuDishes");
        }
    }
}
