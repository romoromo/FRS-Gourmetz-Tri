using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addDOfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LoadingTime",
                table: "DeliveryOrderNews",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivingTIme",
                table: "DeliveryOrderNews",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ReceivedQty",
                table: "DeliveryBentoNews",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoadingTime",
                table: "DeliveryOrderNews");

            migrationBuilder.DropColumn(
                name: "ReceivingTIme",
                table: "DeliveryOrderNews");

            migrationBuilder.DropColumn(
                name: "ReceivedQty",
                table: "DeliveryBentoNews");
        }
    }
}
