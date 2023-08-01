using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addingtimestamptoDO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClosedBy",
                table: "DeliveryOrders",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedDate",
                table: "DeliveryOrders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FoodExpiryDate",
                table: "DeliveryOrders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrders_ClosedBy",
                table: "DeliveryOrders",
                column: "ClosedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryOrders_User_ClosedBy",
                table: "DeliveryOrders",
                column: "ClosedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryOrders_User_ClosedBy",
                table: "DeliveryOrders");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryOrders_ClosedBy",
                table: "DeliveryOrders");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "DeliveryOrders");

            migrationBuilder.DropColumn(
                name: "ClosedDate",
                table: "DeliveryOrders");

            migrationBuilder.DropColumn(
                name: "FoodExpiryDate",
                table: "DeliveryOrders");
        }
    }
}
