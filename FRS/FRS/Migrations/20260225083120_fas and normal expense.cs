using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class fasandnormalexpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "FasExpensed",
                table: "StudentWalletTransactions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FasTopup",
                table: "StudentWalletTransactions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "NormalExpensed",
                table: "StudentWalletTransactions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "NormalTopup",
                table: "StudentWalletTransactions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "StudentWalletTransactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FasExpensed",
                table: "StudentWalletTransactions");

            migrationBuilder.DropColumn(
                name: "FasTopup",
                table: "StudentWalletTransactions");

            migrationBuilder.DropColumn(
                name: "NormalExpensed",
                table: "StudentWalletTransactions");

            migrationBuilder.DropColumn(
                name: "NormalTopup",
                table: "StudentWalletTransactions");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "StudentWalletTransactions");
        }
    }
}
