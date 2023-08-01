using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_new_columns_user : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserClaim_User_UserId",
                table: "UserClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_User_UserId",
                table: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_User_InstitutionId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Role_InstitutionId",
                table: "Role");

            migrationBuilder.AddColumn<string>(
                name: "HomeNo",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Institutions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_User_InstitutionId_Email",
                table: "User",
                columns: new[] { "InstitutionId", "Email" },
                unique: true,
                filter: "[InstitutionId] IS NOT NULL AND [Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Role_InstitutionId_Name",
                table: "Role",
                columns: new[] { "InstitutionId", "Name" },
                unique: true,
                filter: "[InstitutionId] IS NOT NULL AND [Name] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaim_User_UserId",
                table: "UserClaim",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_User_UserId",
                table: "UserRole",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserClaim_User_UserId",
                table: "UserClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_User_UserId",
                table: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_User_InstitutionId_Email",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Role_InstitutionId_Name",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "HomeNo",
                table: "User");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Institutions");

            migrationBuilder.CreateIndex(
                name: "IX_User_InstitutionId",
                table: "User",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_InstitutionId",
                table: "Role",
                column: "InstitutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaim_User_UserId",
                table: "UserClaim",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_User_UserId",
                table: "UserRole",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
