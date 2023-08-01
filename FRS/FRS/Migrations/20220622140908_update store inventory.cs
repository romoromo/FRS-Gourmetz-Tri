using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class updatestoreinventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "StoreInventoryDetails",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeReceived",
                table: "StoreInventoryDetails",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "StoreInventoryDetails");

            migrationBuilder.DropColumn(
                name: "TimeReceived",
                table: "StoreInventoryDetails");
        }
    }
}
