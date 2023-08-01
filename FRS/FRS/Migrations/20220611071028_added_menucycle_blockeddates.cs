using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_menucycle_blockeddates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuCycleBlockedDates",
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
                    EffectiveDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCycleBlockedDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuCycleBlockedDates_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuCycleBlockedDates_MenuCycles_MenuCycleId",
                        column: x => x.MenuCycleId,
                        principalTable: "MenuCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuCycleBlockedDates_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleBlockedDates_CreatedBy",
                table: "MenuCycleBlockedDates",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleBlockedDates_MenuCycleId",
                table: "MenuCycleBlockedDates",
                column: "MenuCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleBlockedDates_UpdatedBy",
                table: "MenuCycleBlockedDates",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuCycleBlockedDates");
        }
    }
}
