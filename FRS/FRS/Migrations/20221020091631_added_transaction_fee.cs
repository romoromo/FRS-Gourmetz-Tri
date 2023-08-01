using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_transaction_fee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Label",
                table: "Waivers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransactionFeeId",
                table: "Waivers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Waivers_TransactionFeeId",
                table: "Waivers",
                column: "TransactionFeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Waivers_TransactionFees_TransactionFeeId",
                table: "Waivers",
                column: "TransactionFeeId",
                principalTable: "TransactionFees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Waivers_TransactionFees_TransactionFeeId",
                table: "Waivers");

            migrationBuilder.DropIndex(
                name: "IX_Waivers_TransactionFeeId",
                table: "Waivers");

            migrationBuilder.DropColumn(
                name: "Label",
                table: "Waivers");

            migrationBuilder.DropColumn(
                name: "TransactionFeeId",
                table: "Waivers");
        }
    }
}
