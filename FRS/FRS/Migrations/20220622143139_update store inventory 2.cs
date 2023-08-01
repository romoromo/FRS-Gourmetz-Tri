using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class updatestoreinventory2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreInfoId",
                table: "StoreInventoryDetails",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StoreInventoryDetails_StoreInfoId",
                table: "StoreInventoryDetails",
                column: "StoreInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreInventoryDetails_StoreInfos_StoreInfoId",
                table: "StoreInventoryDetails",
                column: "StoreInfoId",
                principalTable: "StoreInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreInventoryDetails_StoreInfos_StoreInfoId",
                table: "StoreInventoryDetails");

            migrationBuilder.DropIndex(
                name: "IX_StoreInventoryDetails_StoreInfoId",
                table: "StoreInventoryDetails");

            migrationBuilder.DropColumn(
                name: "StoreInfoId",
                table: "StoreInventoryDetails");
        }
    }
}
