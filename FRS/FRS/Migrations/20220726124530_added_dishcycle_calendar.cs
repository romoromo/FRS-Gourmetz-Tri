using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_dishcycle_calendar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DishCycleCalendarId",
                table: "MenuCycleCalendarBlockedDates",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DishCycleCalendars",
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
                    DishCycleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishCycleCalendars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishCycleCalendars_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishCycleCalendars_DishCycles_DishCycleId",
                        column: x => x.DishCycleId,
                        principalTable: "DishCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishCycleCalendars_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DishCycleCalendarBlockedDates",
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
                    DishCycleCalendarId = table.Column<int>(nullable: true),
                    EffectiveDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishCycleCalendarBlockedDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishCycleCalendarBlockedDates_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishCycleCalendarBlockedDates_DishCycleCalendars_DishCycleCalendarId",
                        column: x => x.DishCycleCalendarId,
                        principalTable: "DishCycleCalendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishCycleCalendarBlockedDates_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycleCalendarBlockedDates_DishCycleCalendarId",
                table: "MenuCycleCalendarBlockedDates",
                column: "DishCycleCalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleCalendarBlockedDates_CreatedBy",
                table: "DishCycleCalendarBlockedDates",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleCalendarBlockedDates_DishCycleCalendarId",
                table: "DishCycleCalendarBlockedDates",
                column: "DishCycleCalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleCalendarBlockedDates_UpdatedBy",
                table: "DishCycleCalendarBlockedDates",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleCalendars_CreatedBy",
                table: "DishCycleCalendars",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleCalendars_DishCycleId",
                table: "DishCycleCalendars",
                column: "DishCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleCalendars_UpdatedBy",
                table: "DishCycleCalendars",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycleCalendarBlockedDates_DishCycleCalendars_DishCycleCalendarId",
                table: "MenuCycleCalendarBlockedDates",
                column: "DishCycleCalendarId",
                principalTable: "DishCycleCalendars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycleCalendarBlockedDates_DishCycleCalendars_DishCycleCalendarId",
                table: "MenuCycleCalendarBlockedDates");

            migrationBuilder.DropTable(
                name: "DishCycleCalendarBlockedDates");

            migrationBuilder.DropTable(
                name: "DishCycleCalendars");

            migrationBuilder.DropIndex(
                name: "IX_MenuCycleCalendarBlockedDates_DishCycleCalendarId",
                table: "MenuCycleCalendarBlockedDates");

            migrationBuilder.DropColumn(
                name: "DishCycleCalendarId",
                table: "MenuCycleCalendarBlockedDates");
        }
    }
}
