using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_mealsession_fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CalSourceTime",
                table: "MealSessionDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MealSessionName",
                table: "MealSessionDetails",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OverheadTime",
                table: "MealSessionDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouteId",
                table: "MealSessionDetails",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RouteTime",
                table: "MealSessionDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealSessionDetails_RouteId",
                table: "MealSessionDetails",
                column: "RouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_MealSessionDetails_Routes_RouteId",
                table: "MealSessionDetails",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealSessionDetails_Routes_RouteId",
                table: "MealSessionDetails");

            migrationBuilder.DropIndex(
                name: "IX_MealSessionDetails_RouteId",
                table: "MealSessionDetails");

            migrationBuilder.DropColumn(
                name: "CalSourceTime",
                table: "MealSessionDetails");

            migrationBuilder.DropColumn(
                name: "MealSessionName",
                table: "MealSessionDetails");

            migrationBuilder.DropColumn(
                name: "OverheadTime",
                table: "MealSessionDetails");

            migrationBuilder.DropColumn(
                name: "RouteId",
                table: "MealSessionDetails");

            migrationBuilder.DropColumn(
                name: "RouteTime",
                table: "MealSessionDetails");
        }
    }
}
