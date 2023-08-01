using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class add_dishcyclemenutable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishCycleScheduleDetails_Dishes_DishId",
                table: "DishCycleScheduleDetails");

            migrationBuilder.DropIndex(
                name: "IX_DishCycleScheduleDetails_DishId",
                table: "DishCycleScheduleDetails");

            migrationBuilder.DropColumn(
                name: "DishId",
                table: "DishCycleScheduleDetails");

            migrationBuilder.CreateTable(
                name: "DishCycleScheduleDetailMenus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DishCycleScheduleDetailId = table.Column<int>(nullable: false),
                    DishId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishCycleScheduleDetailMenus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishCycleScheduleDetailMenus_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishCycleScheduleDetailMenus_DishCycleScheduleDetails_DishCycleScheduleDetailId",
                        column: x => x.DishCycleScheduleDetailId,
                        principalTable: "DishCycleScheduleDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishCycleScheduleDetailMenus_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishCycleScheduleDetailMenus_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleDetailMenus_CreatedBy",
                table: "DishCycleScheduleDetailMenus",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleDetailMenus_DishCycleScheduleDetailId",
                table: "DishCycleScheduleDetailMenus",
                column: "DishCycleScheduleDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleDetailMenus_DishId",
                table: "DishCycleScheduleDetailMenus",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleDetailMenus_UpdatedBy",
                table: "DishCycleScheduleDetailMenus",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DishCycleScheduleDetailMenus");

            migrationBuilder.AddColumn<int>(
                name: "DishId",
                table: "DishCycleScheduleDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleDetails_DishId",
                table: "DishCycleScheduleDetails",
                column: "DishId");

            migrationBuilder.AddForeignKey(
                name: "FK_DishCycleScheduleDetails_Dishes_DishId",
                table: "DishCycleScheduleDetails",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
