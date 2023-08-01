using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class link_outletprofile_classlevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OutletProfileId",
                table: "ClassLevels",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevels_OutletProfileId",
                table: "ClassLevels",
                column: "OutletProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevels_OutletProfiles_OutletProfileId",
                table: "ClassLevels",
                column: "OutletProfileId",
                principalTable: "OutletProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevels_OutletProfiles_OutletProfileId",
                table: "ClassLevels");

            migrationBuilder.DropIndex(
                name: "IX_ClassLevels_OutletProfileId",
                table: "ClassLevels");

            migrationBuilder.DropColumn(
                name: "OutletProfileId",
                table: "ClassLevels");
        }
    }
}
