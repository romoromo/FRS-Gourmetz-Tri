using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class change_outletprofile_tooutletinstudents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevels_OutletProfiles_OutletProfileId",
                table: "ClassLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_OutletProfiles_OutletProfileId",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "OutletProfileId",
                table: "Students",
                newName: "OutletId");

            migrationBuilder.RenameIndex(
                name: "IX_Students_OutletProfileId",
                table: "Students",
                newName: "IX_Students_OutletId");

            migrationBuilder.RenameColumn(
                name: "OutletProfileId",
                table: "ClassLevels",
                newName: "OutletId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassLevels_OutletProfileId",
                table: "ClassLevels",
                newName: "IX_ClassLevels_OutletId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevels_Outlets_OutletId",
                table: "ClassLevels",
                column: "OutletId",
                principalTable: "Outlets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Outlets_OutletId",
                table: "Students",
                column: "OutletId",
                principalTable: "Outlets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevels_Outlets_OutletId",
                table: "ClassLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Outlets_OutletId",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "OutletId",
                table: "Students",
                newName: "OutletProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Students_OutletId",
                table: "Students",
                newName: "IX_Students_OutletProfileId");

            migrationBuilder.RenameColumn(
                name: "OutletId",
                table: "ClassLevels",
                newName: "OutletProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassLevels_OutletId",
                table: "ClassLevels",
                newName: "IX_ClassLevels_OutletProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevels_OutletProfiles_OutletProfileId",
                table: "ClassLevels",
                column: "OutletProfileId",
                principalTable: "OutletProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_OutletProfiles_OutletProfileId",
                table: "Students",
                column: "OutletProfileId",
                principalTable: "OutletProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
