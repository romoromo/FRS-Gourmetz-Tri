using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addreturntotokenorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BentoCode",
                table: "TokenOrders",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnTime",
                table: "TokenOrders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BentoCode",
                table: "TokenOrders");

            migrationBuilder.DropColumn(
                name: "ReturnTime",
                table: "TokenOrders");
        }
    }
}
