using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationPOSwithPayment2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_POSSales_PaymentID",
                table: "POSSales");

            migrationBuilder.CreateIndex(
                name: "IX_POSSales_PaymentID",
                table: "POSSales",
                column: "PaymentID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_POSSales_PaymentID",
                table: "POSSales");

            migrationBuilder.CreateIndex(
                name: "IX_POSSales_PaymentID",
                table: "POSSales",
                column: "PaymentID");
        }
    }
}
