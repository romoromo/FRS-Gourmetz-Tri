using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addCartonAssetAndDish : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CartonAssetId",
                table: "BentoAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DishId",
                table: "BentoAssets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BentoAssets_CartonAssetId",
                table: "BentoAssets",
                column: "CartonAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_BentoAssets_DishId",
                table: "BentoAssets",
                column: "DishId");

            migrationBuilder.AddForeignKey(
                name: "FK_BentoAssets_CartonAssets_CartonAssetId",
                table: "BentoAssets",
                column: "CartonAssetId",
                principalTable: "CartonAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BentoAssets_Dishes_DishId",
                table: "BentoAssets",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BentoAssets_CartonAssets_CartonAssetId",
                table: "BentoAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_BentoAssets_Dishes_DishId",
                table: "BentoAssets");

            migrationBuilder.DropIndex(
                name: "IX_BentoAssets_CartonAssetId",
                table: "BentoAssets");

            migrationBuilder.DropIndex(
                name: "IX_BentoAssets_DishId",
                table: "BentoAssets");

            migrationBuilder.DropColumn(
                name: "CartonAssetId",
                table: "BentoAssets");

            migrationBuilder.DropColumn(
                name: "DishId",
                table: "BentoAssets");
        }
    }
}
