using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class update_user_alert : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserOrderAlert_User_CreatedBy",
                table: "UserOrderAlert");

            migrationBuilder.DropForeignKey(
                name: "FK_UserOrderAlert_TokenOrders_TokenOrderId",
                table: "UserOrderAlert");

            migrationBuilder.DropForeignKey(
                name: "FK_UserOrderAlert_User_UpdatedBy",
                table: "UserOrderAlert");

            migrationBuilder.DropForeignKey(
                name: "FK_UserOrderAlert_User_UserId",
                table: "UserOrderAlert");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserOrderAlert",
                table: "UserOrderAlert");

            migrationBuilder.DropIndex(
                name: "IX_UserOrderAlert_TokenOrderId",
                table: "UserOrderAlert");

            migrationBuilder.DropColumn(
                name: "IsAbandonedCart1Sent",
                table: "UserOrderAlert");

            migrationBuilder.DropColumn(
                name: "IsAbandonedCart2Sent",
                table: "UserOrderAlert");

            migrationBuilder.DropColumn(
                name: "TokenOrderId",
                table: "UserOrderAlert");

            migrationBuilder.RenameTable(
                name: "UserOrderAlert",
                newName: "UserOrderAlerts");

            migrationBuilder.RenameIndex(
                name: "IX_UserOrderAlert_UserId",
                table: "UserOrderAlerts",
                newName: "IX_UserOrderAlerts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserOrderAlert_UpdatedBy",
                table: "UserOrderAlerts",
                newName: "IX_UserOrderAlerts_UpdatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_UserOrderAlert_CreatedBy",
                table: "UserOrderAlerts",
                newName: "IX_UserOrderAlerts_CreatedBy");

            migrationBuilder.AddColumn<bool>(
                name: "isNotifMissedCollection",
                table: "Students",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "AbandonedCart1SentDate",
                table: "UserOrderAlerts",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AbandonedCart2SentDate",
                table: "UserOrderAlerts",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MissedCollectedSentDate",
                table: "UserOrderAlerts",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NoOrderNextWeekSentDate",
                table: "UserOrderAlerts",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserOrderAlerts",
                table: "UserOrderAlerts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOrderAlerts_User_CreatedBy",
                table: "UserOrderAlerts",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserOrderAlerts_User_UpdatedBy",
                table: "UserOrderAlerts",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserOrderAlerts_User_UserId",
                table: "UserOrderAlerts",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserOrderAlerts_User_CreatedBy",
                table: "UserOrderAlerts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserOrderAlerts_User_UpdatedBy",
                table: "UserOrderAlerts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserOrderAlerts_User_UserId",
                table: "UserOrderAlerts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserOrderAlerts",
                table: "UserOrderAlerts");

            migrationBuilder.DropColumn(
                name: "isNotifMissedCollection",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "AbandonedCart1SentDate",
                table: "UserOrderAlerts");

            migrationBuilder.DropColumn(
                name: "AbandonedCart2SentDate",
                table: "UserOrderAlerts");

            migrationBuilder.DropColumn(
                name: "MissedCollectedSentDate",
                table: "UserOrderAlerts");

            migrationBuilder.DropColumn(
                name: "NoOrderNextWeekSentDate",
                table: "UserOrderAlerts");

            migrationBuilder.RenameTable(
                name: "UserOrderAlerts",
                newName: "UserOrderAlert");

            migrationBuilder.RenameIndex(
                name: "IX_UserOrderAlerts_UserId",
                table: "UserOrderAlert",
                newName: "IX_UserOrderAlert_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserOrderAlerts_UpdatedBy",
                table: "UserOrderAlert",
                newName: "IX_UserOrderAlert_UpdatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_UserOrderAlerts_CreatedBy",
                table: "UserOrderAlert",
                newName: "IX_UserOrderAlert_CreatedBy");

            migrationBuilder.AddColumn<bool>(
                name: "IsAbandonedCart1Sent",
                table: "UserOrderAlert",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAbandonedCart2Sent",
                table: "UserOrderAlert",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TokenOrderId",
                table: "UserOrderAlert",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserOrderAlert",
                table: "UserOrderAlert",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrderAlert_TokenOrderId",
                table: "UserOrderAlert",
                column: "TokenOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOrderAlert_User_CreatedBy",
                table: "UserOrderAlert",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserOrderAlert_TokenOrders_TokenOrderId",
                table: "UserOrderAlert",
                column: "TokenOrderId",
                principalTable: "TokenOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserOrderAlert_User_UpdatedBy",
                table: "UserOrderAlert",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserOrderAlert_User_UserId",
                table: "UserOrderAlert",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
