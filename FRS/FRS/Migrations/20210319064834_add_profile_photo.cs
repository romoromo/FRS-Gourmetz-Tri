using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class add_profile_photo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "User",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_FileId",
                table: "User",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Files_FileId",
                table: "User",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Files_FileId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_FileId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "User");
        }
    }
}
