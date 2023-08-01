using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class makeAssetTypeNotMandatory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetModels_AssetTypes_AssetTypeId",
                table: "AssetModels");

            migrationBuilder.AlterColumn<int>(
                name: "AssetTypeId",
                table: "AssetModels",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_AssetModels_AssetTypes_AssetTypeId",
                table: "AssetModels",
                column: "AssetTypeId",
                principalTable: "AssetTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetModels_AssetTypes_AssetTypeId",
                table: "AssetModels");

            migrationBuilder.AlterColumn<int>(
                name: "AssetTypeId",
                table: "AssetModels",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetModels_AssetTypes_AssetTypeId",
                table: "AssetModels",
                column: "AssetTypeId",
                principalTable: "AssetTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
