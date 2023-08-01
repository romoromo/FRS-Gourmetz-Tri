using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class DOandassetupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MealSessionDetailId",
                table: "DeliveryOrderNews",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MealSessionId",
                table: "DeliveryOrderNews",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouteId",
                table: "DeliveryOrderNews",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleNumber",
                table: "DeliveryOrderNews",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLoad",
                table: "DeliveryDetailNews",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LoadingTime",
                table: "DeliveryDetailNews",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "BentoAssets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrderNews_MealSessionId",
                table: "DeliveryOrderNews",
                column: "MealSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrderNews_RouteId",
                table: "DeliveryOrderNews",
                column: "RouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryOrderNews_MealSessionDetails_MealSessionId",
                table: "DeliveryOrderNews",
                column: "MealSessionId",
                principalTable: "MealSessionDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryOrderNews_Routes_RouteId",
                table: "DeliveryOrderNews",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryOrderNews_MealSessionDetails_MealSessionId",
                table: "DeliveryOrderNews");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryOrderNews_Routes_RouteId",
                table: "DeliveryOrderNews");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryOrderNews_MealSessionId",
                table: "DeliveryOrderNews");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryOrderNews_RouteId",
                table: "DeliveryOrderNews");

            migrationBuilder.DropColumn(
                name: "MealSessionDetailId",
                table: "DeliveryOrderNews");

            migrationBuilder.DropColumn(
                name: "MealSessionId",
                table: "DeliveryOrderNews");

            migrationBuilder.DropColumn(
                name: "RouteId",
                table: "DeliveryOrderNews");

            migrationBuilder.DropColumn(
                name: "VehicleNumber",
                table: "DeliveryOrderNews");

            migrationBuilder.DropColumn(
                name: "IsLoad",
                table: "DeliveryDetailNews");

            migrationBuilder.DropColumn(
                name: "LoadingTime",
                table: "DeliveryDetailNews");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "BentoAssets");
        }
    }
}
