using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class updateOrderHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TokenOrdereds_TokensOrderHistorys_TokensOrderHistoryId",
                table: "TokenOrdereds");

            migrationBuilder.DropIndex(
                name: "IX_TokenOrdereds_TokensOrderHistoryId",
                table: "TokenOrdereds");

            migrationBuilder.DropColumn(
                name: "TokensOrderHistoryId",
                table: "TokenOrdereds");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TokensOrderHistoryId",
                table: "TokenOrdereds",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrdereds_TokensOrderHistoryId",
                table: "TokenOrdereds",
                column: "TokensOrderHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TokenOrdereds_TokensOrderHistorys_TokensOrderHistoryId",
                table: "TokenOrdereds",
                column: "TokensOrderHistoryId",
                principalTable: "TokensOrderHistorys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
