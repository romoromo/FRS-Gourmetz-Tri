using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_outletprofile_menucycle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OutletProfileId",
                table: "MenuCycles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OutletProfileId",
                table: "MealPeriods",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycles_OutletProfileId",
                table: "MenuCycles",
                column: "OutletProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPeriods_OutletProfileId",
                table: "MealPeriods",
                column: "OutletProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_MealPeriods_OutletProfiles_OutletProfileId",
                table: "MealPeriods",
                column: "OutletProfileId",
                principalTable: "OutletProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycles_OutletProfiles_OutletProfileId",
                table: "MenuCycles",
                column: "OutletProfileId",
                principalTable: "OutletProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealPeriods_OutletProfiles_OutletProfileId",
                table: "MealPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycles_OutletProfiles_OutletProfileId",
                table: "MenuCycles");

            migrationBuilder.DropIndex(
                name: "IX_MenuCycles_OutletProfileId",
                table: "MenuCycles");

            migrationBuilder.DropIndex(
                name: "IX_MealPeriods_OutletProfileId",
                table: "MealPeriods");

            migrationBuilder.DropColumn(
                name: "OutletProfileId",
                table: "MenuCycles");

            migrationBuilder.DropColumn(
                name: "OutletProfileId",
                table: "MealPeriods");
        }
    }
}
