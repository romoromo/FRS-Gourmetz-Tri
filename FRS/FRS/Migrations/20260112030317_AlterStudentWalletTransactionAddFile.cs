using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AlterStudentWalletTransactionAddFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "StudentWalletTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentWalletTransactions_FileId",
                table: "StudentWalletTransactions",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentWalletTransactions_Files_FileId",
                table: "StudentWalletTransactions",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentWalletTransactions_Files_FileId",
                table: "StudentWalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_StudentWalletTransactions_FileId",
                table: "StudentWalletTransactions");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "StudentWalletTransactions");
        }
    }
}
