using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class allocationlink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TokenLabels_order_id",
                table: "TokenLabels",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_TokenDishLabels_dish_id",
                table: "TokenDishLabels",
                column: "dish_id");

            migrationBuilder.AddForeignKey(
                name: "FK_TokenDishLabels_Dishes_dish_id",
                table: "TokenDishLabels",
                column: "dish_id",
                principalTable: "Dishes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TokenLabels_TokenOrders_order_id",
                table: "TokenLabels",
                column: "order_id",
                principalTable: "TokenOrders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TokenDishLabels_Dishes_dish_id",
                table: "TokenDishLabels");

            migrationBuilder.DropForeignKey(
                name: "FK_TokenLabels_TokenOrders_order_id",
                table: "TokenLabels");

            migrationBuilder.DropIndex(
                name: "IX_TokenLabels_order_id",
                table: "TokenLabels");

            migrationBuilder.DropIndex(
                name: "IX_TokenDishLabels_dish_id",
                table: "TokenDishLabels");
        }
    }
}
