using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexOnStudentWallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentWallets_StudentId",
                table: "StudentWallets");

            migrationBuilder.CreateIndex(
                name: "IX_StudentWallets_StudentId_Type",
                table: "StudentWallets",
                columns: new[] { "StudentId", "Type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentWallets_StudentId_Type",
                table: "StudentWallets");

            migrationBuilder.CreateIndex(
                name: "IX_StudentWallets_StudentId",
                table: "StudentWallets",
                column: "StudentId");
        }
    }
}
