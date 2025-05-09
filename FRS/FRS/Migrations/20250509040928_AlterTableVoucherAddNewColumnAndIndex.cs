using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableVoucherAddNewColumnAndIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsageQuantityUsed",
                table: "Vouchers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_EndDateTime",
                table: "Vouchers",
                column: "EndDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_OutletProfileId",
                table: "Vouchers",
                column: "OutletProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_StartDateTime",
                table: "Vouchers",
                column: "StartDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_StartDateTime_EndDateTime",
                table: "Vouchers",
                columns: new[] { "StartDateTime", "EndDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_UsageQuantity",
                table: "Vouchers",
                column: "UsageQuantity");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_UsageQuantityUsed",
                table: "Vouchers",
                column: "UsageQuantityUsed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vouchers_EndDateTime",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_OutletProfileId",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_StartDateTime",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_StartDateTime_EndDateTime",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_UsageQuantity",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_UsageQuantityUsed",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "UsageQuantityUsed",
                table: "Vouchers");
        }
    }
}
