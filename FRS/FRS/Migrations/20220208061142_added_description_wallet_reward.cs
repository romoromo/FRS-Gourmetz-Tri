using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_description_wallet_reward : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "WalletTransactions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Wallets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "RewardTransactions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Rewards",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "RewardTransactions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Rewards");
        }
    }
}
