using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class link_outletprofile_student : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OutletProfileId",
                table: "Students",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_OutletProfileId",
                table: "Students",
                column: "OutletProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_OutletProfiles_OutletProfileId",
                table: "Students",
                column: "OutletProfileId",
                principalTable: "OutletProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_OutletProfiles_OutletProfileId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_OutletProfileId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "OutletProfileId",
                table: "Students");
        }
    }
}
