using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationPOSwithPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_POSSales_PaymentID",
                table: "POSSales",
                column: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_POSSales_Payments_PaymentID",
                table: "POSSales",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_POSSales_Payments_PaymentID",
                table: "POSSales");

            migrationBuilder.DropIndex(
                name: "IX_POSSales_PaymentID",
                table: "POSSales");
        }
    }
}
