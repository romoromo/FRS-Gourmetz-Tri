using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class removed_unique_index_user : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_InstitutionId_Email",
                table: "User");

            migrationBuilder.CreateIndex(
                name: "IX_User_InstitutionId",
                table: "User",
                column: "InstitutionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_InstitutionId",
                table: "User");

            migrationBuilder.CreateIndex(
                name: "IX_User_InstitutionId_Email",
                table: "User",
                columns: new[] { "InstitutionId", "Email" },
                unique: true,
                filter: "[InstitutionId] IS NOT NULL AND [Email] IS NOT NULL");
        }
    }
}
