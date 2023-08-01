using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addStoreToBento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreInfoId",
                table: "BentoAssets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BentoAssets_StoreInfoId",
                table: "BentoAssets",
                column: "StoreInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_BentoAssets_StoreInfos_StoreInfoId",
                table: "BentoAssets",
                column: "StoreInfoId",
                principalTable: "StoreInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BentoAssets_StoreInfos_StoreInfoId",
                table: "BentoAssets");

            migrationBuilder.DropIndex(
                name: "IX_BentoAssets_StoreInfoId",
                table: "BentoAssets");

            migrationBuilder.DropColumn(
                name: "StoreInfoId",
                table: "BentoAssets");
        }
    }
}
