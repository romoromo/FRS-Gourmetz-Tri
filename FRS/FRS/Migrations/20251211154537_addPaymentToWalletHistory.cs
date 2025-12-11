using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class addPaymentToWalletHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "StudentWalletTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TokenOrderId",
                table: "StudentWalletTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentWalletTransactions_PaymentId",
                table: "StudentWalletTransactions",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentWalletTransactions_TokenOrderId",
                table: "StudentWalletTransactions",
                column: "TokenOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentWalletTransactions_Payments_PaymentId",
                table: "StudentWalletTransactions",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentWalletTransactions_TokenOrders_TokenOrderId",
                table: "StudentWalletTransactions",
                column: "TokenOrderId",
                principalTable: "TokenOrders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentWalletTransactions_Payments_PaymentId",
                table: "StudentWalletTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentWalletTransactions_TokenOrders_TokenOrderId",
                table: "StudentWalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_StudentWalletTransactions_PaymentId",
                table: "StudentWalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_StudentWalletTransactions_TokenOrderId",
                table: "StudentWalletTransactions");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "StudentWalletTransactions");

            migrationBuilder.DropColumn(
                name: "TokenOrderId",
                table: "StudentWalletTransactions");
        }
    }
}
