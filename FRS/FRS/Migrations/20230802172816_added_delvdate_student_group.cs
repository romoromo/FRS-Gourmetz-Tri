using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_delvdate_student_group : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryEndDate",
                table: "StudentGroups",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryStartDate",
                table: "StudentGroups",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MealSessionId",
                table: "StudentGroups",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroups_MealSessionId",
                table: "StudentGroups",
                column: "MealSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentGroups_MealSessions_MealSessionId",
                table: "StudentGroups",
                column: "MealSessionId",
                principalTable: "MealSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentGroups_MealSessions_MealSessionId",
                table: "StudentGroups");

            migrationBuilder.DropIndex(
                name: "IX_StudentGroups_MealSessionId",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "DeliveryEndDate",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "DeliveryStartDate",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "MealSessionId",
                table: "StudentGroups");
        }
    }
}
