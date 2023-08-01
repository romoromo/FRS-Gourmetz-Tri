using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_dishcycleperiod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DishCyclePeriods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DishCycleId = table.Column<int>(nullable: false),
                    MealPeriodId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishCyclePeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishCyclePeriods_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishCyclePeriods_DishCycles_DishCycleId",
                        column: x => x.DishCycleId,
                        principalTable: "DishCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishCyclePeriods_MealPeriods_MealPeriodId",
                        column: x => x.MealPeriodId,
                        principalTable: "MealPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishCyclePeriods_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DishCyclePeriods_CreatedBy",
                table: "DishCyclePeriods",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DishCyclePeriods_DishCycleId",
                table: "DishCyclePeriods",
                column: "DishCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_DishCyclePeriods_MealPeriodId",
                table: "DishCyclePeriods",
                column: "MealPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_DishCyclePeriods_UpdatedBy",
                table: "DishCyclePeriods",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DishCyclePeriods");
        }
    }
}
