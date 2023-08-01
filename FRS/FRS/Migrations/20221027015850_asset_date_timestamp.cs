using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class asset_date_timestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "CartonAssets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStamp",
                table: "CartonAssets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "BentoAssets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStamp",
                table: "BentoAssets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "CartonAssets");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "CartonAssets");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "BentoAssets");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "BentoAssets");
        }
    }
}
