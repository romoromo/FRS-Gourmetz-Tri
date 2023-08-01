using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class store_to_token_order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "TokenOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrders_StoreId",
                table: "TokenOrders",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_TokenOrders_StoreInfos_StoreId",
                table: "TokenOrders",
                column: "StoreId",
                principalTable: "StoreInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TokenOrders_StoreInfos_StoreId",
                table: "TokenOrders");

            migrationBuilder.DropIndex(
                name: "IX_TokenOrders_StoreId",
                table: "TokenOrders");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "TokenOrders");
        }
    }
}
