using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class cartonassetlinkdish : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DishId",
                table: "CartonAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Qty",
                table: "CartonAssets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StoreInfoId",
                table: "CartonAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToStoreInfoId",
                table: "CartonAssets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartonAssets_DishId",
                table: "CartonAssets",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_CartonAssets_StoreInfoId",
                table: "CartonAssets",
                column: "StoreInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartonAssets_Dishes_DishId",
                table: "CartonAssets",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CartonAssets_StoreInfos_StoreInfoId",
                table: "CartonAssets",
                column: "StoreInfoId",
                principalTable: "StoreInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartonAssets_Dishes_DishId",
                table: "CartonAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_CartonAssets_StoreInfos_StoreInfoId",
                table: "CartonAssets");

            migrationBuilder.DropIndex(
                name: "IX_CartonAssets_DishId",
                table: "CartonAssets");

            migrationBuilder.DropIndex(
                name: "IX_CartonAssets_StoreInfoId",
                table: "CartonAssets");

            migrationBuilder.DropColumn(
                name: "DishId",
                table: "CartonAssets");

            migrationBuilder.DropColumn(
                name: "Qty",
                table: "CartonAssets");

            migrationBuilder.DropColumn(
                name: "StoreInfoId",
                table: "CartonAssets");

            migrationBuilder.DropColumn(
                name: "ToStoreInfoId",
                table: "CartonAssets");
        }
    }
}
