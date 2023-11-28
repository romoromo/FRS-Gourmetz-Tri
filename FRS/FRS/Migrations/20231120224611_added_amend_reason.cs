using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_amend_reason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserOutlets_User_UserId",
                table: "UserOutlets");

            migrationBuilder.AddColumn<string>(
                name: "AmendReason",
                table: "TokenOrders",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserOutlets_User_UserId",
                table: "UserOutlets",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserOutlets_User_UserId",
                table: "UserOutlets");

            migrationBuilder.DropColumn(
                name: "AmendReason",
                table: "TokenOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOutlets_User_UserId",
                table: "UserOutlets",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
