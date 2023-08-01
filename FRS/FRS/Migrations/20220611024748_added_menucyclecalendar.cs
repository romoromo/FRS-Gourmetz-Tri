using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_menucyclecalendar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuCycleCalendars",
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
                    MenuCycleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCycleCalendars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuCycleCalendars_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuCycleCalendars_MenuCycles_MenuCycleId",
                        column: x => x.MenuCycleId,
                        principalTable: "MenuCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuCycleCalendars_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MenuCycleCalendarBlockedDates",
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
                    MenuCycleCalendarId = table.Column<int>(nullable: true),
                    EffectiveDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCycleCalendarBlockedDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuCycleCalendarBlockedDates_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuCycleCalendarBlockedDates_MenuCycleCalendars_MenuCycleCalendarId",
                        column: x => x.MenuCycleCalendarId,
                        principalTable: "MenuCycleCalendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuCycleCalendarBlockedDates_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleCalendarBlockedDates_CreatedBy",
                table: "MenuCycleCalendarBlockedDates",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleCalendarBlockedDates_MenuCycleCalendarId",
                table: "MenuCycleCalendarBlockedDates",
                column: "MenuCycleCalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleCalendarBlockedDates_UpdatedBy",
                table: "MenuCycleCalendarBlockedDates",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleCalendars_CreatedBy",
                table: "MenuCycleCalendars",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleCalendars_MenuCycleId",
                table: "MenuCycleCalendars",
                column: "MenuCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleCalendars_UpdatedBy",
                table: "MenuCycleCalendars",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuCycleCalendarBlockedDates");

            migrationBuilder.DropTable(
                name: "MenuCycleCalendars");
        }
    }
}
