using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class update_locationasset_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationAssets_AssetModels_AssetModelId",
                table: "LocationAssets");

            migrationBuilder.DropColumn(
                name: "PurchaseDate",
                table: "LocationAssets");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "LocationAssets");

            migrationBuilder.DropColumn(
                name: "WarrantyEnd",
                table: "LocationAssets");

            migrationBuilder.DropColumn(
                name: "WarrantyStart",
                table: "LocationAssets");

            migrationBuilder.RenameColumn(
                name: "AssetModelId",
                table: "LocationAssets",
                newName: "AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_LocationAssets_AssetModelId",
                table: "LocationAssets",
                newName: "IX_LocationAssets_AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationAssets_Assets_AssetId",
                table: "LocationAssets",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationAssets_Assets_AssetId",
                table: "LocationAssets");

            migrationBuilder.RenameColumn(
                name: "AssetId",
                table: "LocationAssets",
                newName: "AssetModelId");

            migrationBuilder.RenameIndex(
                name: "IX_LocationAssets_AssetId",
                table: "LocationAssets",
                newName: "IX_LocationAssets_AssetModelId");

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchaseDate",
                table: "LocationAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "LocationAssets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WarrantyEnd",
                table: "LocationAssets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WarrantyStart",
                table: "LocationAssets",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationAssets_AssetModels_AssetModelId",
                table: "LocationAssets",
                column: "AssetModelId",
                principalTable: "AssetModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
