using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class bentoAssetandtypeAddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isRFID",
                table: "BentoBoxTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssetRegistrationTime",
                table: "BentoAssets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPackingTime",
                table: "BentoAssets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReturnTime",
                table: "BentoAssets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isRFID",
                table: "BentoBoxTypes");

            migrationBuilder.DropColumn(
                name: "AssetRegistrationTime",
                table: "BentoAssets");

            migrationBuilder.DropColumn(
                name: "LastPackingTime",
                table: "BentoAssets");

            migrationBuilder.DropColumn(
                name: "LastReturnTime",
                table: "BentoAssets");
        }
    }
}
