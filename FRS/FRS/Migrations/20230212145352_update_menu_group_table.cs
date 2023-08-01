using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class update_menu_group_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuGroups_OutletProfiles_OutletProfileId",
                table: "MenuGroups");

            migrationBuilder.RenameColumn(
                name: "OutletProfileId",
                table: "MenuGroups",
                newName: "OutletId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuGroups_OutletProfileId",
                table: "MenuGroups",
                newName: "IX_MenuGroups_OutletId");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "MenuGroups",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "MenuGroups",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "OutletId",
                table: "MenuCycles",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuCycles_OutletId",
                table: "MenuCycles",
                column: "OutletId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCycles_Outlets_OutletId",
                table: "MenuCycles",
                column: "OutletId",
                principalTable: "Outlets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuGroups_Outlets_OutletId",
                table: "MenuGroups",
                column: "OutletId",
                principalTable: "Outlets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCycles_Outlets_OutletId",
                table: "MenuCycles");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuGroups_Outlets_OutletId",
                table: "MenuGroups");

            migrationBuilder.DropIndex(
                name: "IX_MenuCycles_OutletId",
                table: "MenuCycles");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "MenuGroups");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "MenuGroups");

            migrationBuilder.DropColumn(
                name: "OutletId",
                table: "MenuCycles");

            migrationBuilder.RenameColumn(
                name: "OutletId",
                table: "MenuGroups",
                newName: "OutletProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuGroups_OutletId",
                table: "MenuGroups",
                newName: "IX_MenuGroups_OutletProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuGroups_OutletProfiles_OutletProfileId",
                table: "MenuGroups",
                column: "OutletProfileId",
                principalTable: "OutletProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
