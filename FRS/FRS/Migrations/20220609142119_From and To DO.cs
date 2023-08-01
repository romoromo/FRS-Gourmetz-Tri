using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class FromandToDO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FromStoreId",
                table: "DeliveryOrders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToStoreId",
                table: "DeliveryOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrders_FromStoreId",
                table: "DeliveryOrders",
                column: "FromStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrders_ToStoreId",
                table: "DeliveryOrders",
                column: "ToStoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryOrders_StoreInfos_FromStoreId",
                table: "DeliveryOrders",
                column: "FromStoreId",
                principalTable: "StoreInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryOrders_StoreInfos_ToStoreId",
                table: "DeliveryOrders",
                column: "ToStoreId",
                principalTable: "StoreInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryOrders_StoreInfos_FromStoreId",
                table: "DeliveryOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryOrders_StoreInfos_ToStoreId",
                table: "DeliveryOrders");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryOrders_FromStoreId",
                table: "DeliveryOrders");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryOrders_ToStoreId",
                table: "DeliveryOrders");

            migrationBuilder.DropColumn(
                name: "FromStoreId",
                table: "DeliveryOrders");

            migrationBuilder.DropColumn(
                name: "ToStoreId",
                table: "DeliveryOrders");
        }
    }
}
