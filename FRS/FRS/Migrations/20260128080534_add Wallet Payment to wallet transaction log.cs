using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class addWalletPaymenttowallettransactionlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WalletPaymentId",
                table: "StudentWalletTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentWalletTransactions_WalletPaymentId",
                table: "StudentWalletTransactions",
                column: "WalletPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentWalletTransactions_WalletPayments_WalletPaymentId",
                table: "StudentWalletTransactions",
                column: "WalletPaymentId",
                principalTable: "WalletPayments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentWalletTransactions_WalletPayments_WalletPaymentId",
                table: "StudentWalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_StudentWalletTransactions_WalletPaymentId",
                table: "StudentWalletTransactions");

            migrationBuilder.DropColumn(
                name: "WalletPaymentId",
                table: "StudentWalletTransactions");
        }
    }
}
