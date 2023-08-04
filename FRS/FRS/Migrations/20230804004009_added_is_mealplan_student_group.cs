using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_is_mealplan_student_group : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentGroupMealPlans_MealSessions_MealSessionId",
                table: "StudentGroupMealPlans");

            migrationBuilder.DropIndex(
                name: "IX_StudentGroupMealPlans_MealSessionId",
                table: "StudentGroupMealPlans");

            migrationBuilder.AddColumn<bool>(
                name: "IsMealPlan",
                table: "TokenOrders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "StudentGroupMealPlans",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Label",
                table: "StudentGroupMealPlans",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MealSessionDetailId",
                table: "StudentGroupMealPlans",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MealTypeId",
                table: "StudentGroupMealPlans",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Price",
                table: "StudentGroupMealPlans",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "StudentGroupMealPlans",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupMealPlans_MealSessionDetailId",
                table: "StudentGroupMealPlans",
                column: "MealSessionDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupMealPlans_StoreId",
                table: "StudentGroupMealPlans",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentGroupMealPlans_MealSessionDetails_MealSessionDetailId",
                table: "StudentGroupMealPlans",
                column: "MealSessionDetailId",
                principalTable: "MealSessionDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentGroupMealPlans_StoreInfos_StoreId",
                table: "StudentGroupMealPlans",
                column: "StoreId",
                principalTable: "StoreInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentGroupMealPlans_MealSessionDetails_MealSessionDetailId",
                table: "StudentGroupMealPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentGroupMealPlans_StoreInfos_StoreId",
                table: "StudentGroupMealPlans");

            migrationBuilder.DropIndex(
                name: "IX_StudentGroupMealPlans_MealSessionDetailId",
                table: "StudentGroupMealPlans");

            migrationBuilder.DropIndex(
                name: "IX_StudentGroupMealPlans_StoreId",
                table: "StudentGroupMealPlans");

            migrationBuilder.DropColumn(
                name: "IsMealPlan",
                table: "TokenOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "StudentGroupMealPlans");

            migrationBuilder.DropColumn(
                name: "Label",
                table: "StudentGroupMealPlans");

            migrationBuilder.DropColumn(
                name: "MealSessionDetailId",
                table: "StudentGroupMealPlans");

            migrationBuilder.DropColumn(
                name: "MealTypeId",
                table: "StudentGroupMealPlans");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "StudentGroupMealPlans");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "StudentGroupMealPlans");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupMealPlans_MealSessionId",
                table: "StudentGroupMealPlans",
                column: "MealSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentGroupMealPlans_MealSessions_MealSessionId",
                table: "StudentGroupMealPlans",
                column: "MealSessionId",
                principalTable: "MealSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
