using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_photo_column_phonebook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "UserPhonebooks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "ContactGroupMembers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPhonebooks_FileId",
                table: "UserPhonebooks",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactGroupMembers_FileId",
                table: "ContactGroupMembers",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactGroupMembers_Files_FileId",
                table: "ContactGroupMembers",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPhonebooks_Files_FileId",
                table: "UserPhonebooks",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupMembers_Files_FileId",
                table: "ContactGroupMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPhonebooks_Files_FileId",
                table: "UserPhonebooks");

            migrationBuilder.DropIndex(
                name: "IX_UserPhonebooks_FileId",
                table: "UserPhonebooks");

            migrationBuilder.DropIndex(
                name: "IX_ContactGroupMembers_FileId",
                table: "ContactGroupMembers");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "UserPhonebooks");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "ContactGroupMembers");
        }
    }
}
