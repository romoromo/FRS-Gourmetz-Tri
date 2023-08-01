using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_user_id_contact_group_member : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ContactGroupMembers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactGroupMembers_UserId",
                table: "ContactGroupMembers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactGroupMembers_User_UserId",
                table: "ContactGroupMembers",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupMembers_User_UserId",
                table: "ContactGroupMembers");

            migrationBuilder.DropIndex(
                name: "IX_ContactGroupMembers_UserId",
                table: "ContactGroupMembers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ContactGroupMembers");
        }
    }
}
