using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnMaxRedeemCheckoutinVouhcer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxRedeemCheckout",
                table: "Vouchers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MaxRedeemCheckoutMessage",
                table: "Vouchers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_MaxRedeemCheckout",
                table: "Vouchers",
                column: "MaxRedeemCheckout");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vouchers_MaxRedeemCheckout",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "MaxRedeemCheckout",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "MaxRedeemCheckoutMessage",
                table: "Vouchers");
        }
    }
}
