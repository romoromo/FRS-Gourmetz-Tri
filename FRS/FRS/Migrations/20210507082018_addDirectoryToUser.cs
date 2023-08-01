using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addDirectoryToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DirectoryListingId",
                table: "User",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_DirectoryListingId",
                table: "User",
                column: "DirectoryListingId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_DirectoryListing_DirectoryListingId",
                table: "User",
                column: "DirectoryListingId",
                principalTable: "DirectoryListing",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_DirectoryListing_DirectoryListingId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_DirectoryListingId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "DirectoryListingId",
                table: "User");
        }
    }
}
