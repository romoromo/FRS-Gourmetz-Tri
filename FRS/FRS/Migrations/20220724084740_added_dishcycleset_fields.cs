using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_dishcycleset_fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DishCycleScheduleSets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    DishCycleTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishCycleScheduleSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishCycleScheduleSets_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishCycleScheduleSets_DishCycles_DishCycleTypeId",
                        column: x => x.DishCycleTypeId,
                        principalTable: "DishCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishCycleScheduleSets_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleSets_CreatedBy",
                table: "DishCycleScheduleSets",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleSets_DishCycleTypeId",
                table: "DishCycleScheduleSets",
                column: "DishCycleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleSets_UpdatedBy",
                table: "DishCycleScheduleSets",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DishCycleScheduleSets");
        }
    }
}
