using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class directory_floor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FloorId",
                table: "DirectoryListing",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DirectoryListing_FloorId",
                table: "DirectoryListing",
                column: "FloorId");

            migrationBuilder.AddForeignKey(
                name: "FK_DirectoryListing_Floor_FloorId",
                table: "DirectoryListing",
                column: "FloorId",
                principalTable: "Floor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DirectoryListing_Floor_FloorId",
                table: "DirectoryListing");

            migrationBuilder.DropIndex(
                name: "IX_DirectoryListing_FloorId",
                table: "DirectoryListing");

            migrationBuilder.DropColumn(
                name: "FloorId",
                table: "DirectoryListing");
        }
    }
}
