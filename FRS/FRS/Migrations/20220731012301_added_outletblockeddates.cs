using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_outletblockeddates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutletDishBlockedDates",
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
                    MenuCycleId = table.Column<int>(nullable: true),
                    EffectiveDate = table.Column<DateTime>(nullable: false),
                    OutletId = table.Column<int>(nullable: true),
                    DishCycleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletDishBlockedDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutletDishBlockedDates_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletDishBlockedDates_DishCycles_DishCycleId",
                        column: x => x.DishCycleId,
                        principalTable: "DishCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletDishBlockedDates_Outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletDishBlockedDates_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutletDishBlockedDates_CreatedBy",
                table: "OutletDishBlockedDates",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutletDishBlockedDates_DishCycleId",
                table: "OutletDishBlockedDates",
                column: "DishCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletDishBlockedDates_OutletId",
                table: "OutletDishBlockedDates",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletDishBlockedDates_UpdatedBy",
                table: "OutletDishBlockedDates",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutletDishBlockedDates");
        }
    }
}
