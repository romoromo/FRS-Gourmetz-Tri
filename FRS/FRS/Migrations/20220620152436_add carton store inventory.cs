using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addcartonstoreinventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CartonId",
                table: "StoreInventoryDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreInventoryDetails_CartonId",
                table: "StoreInventoryDetails",
                column: "CartonId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreInventoryDetails_CartonAssets_CartonId",
                table: "StoreInventoryDetails",
                column: "CartonId",
                principalTable: "CartonAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreInventoryDetails_CartonAssets_CartonId",
                table: "StoreInventoryDetails");

            migrationBuilder.DropIndex(
                name: "IX_StoreInventoryDetails_CartonId",
                table: "StoreInventoryDetails");

            migrationBuilder.DropColumn(
                name: "CartonId",
                table: "StoreInventoryDetails");
        }
    }
}
