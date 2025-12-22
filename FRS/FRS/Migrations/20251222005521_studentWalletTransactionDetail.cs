using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class studentWalletTransactionDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevelScheduleItems_MealSessionDetails_MealSessionId",
                table: "ClassLevelScheduleItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevelScheduleItems_MealSessions_MealPeriodId",
                table: "ClassLevelScheduleItems");

            migrationBuilder.DropIndex(
                name: "IX_ClassLevelScheduleItems_MealPeriodId",
                table: "ClassLevelScheduleItems");

            migrationBuilder.DropIndex(
                name: "IX_ClassLevelScheduleItems_MealSessionId",
                table: "ClassLevelScheduleItems");

            migrationBuilder.DropColumn(
                name: "MealPeriodId",
                table: "ClassLevelScheduleItems");

            migrationBuilder.DropColumn(
                name: "MealSessionId",
                table: "ClassLevelScheduleItems");

            migrationBuilder.CreateTable(
                name: "StudentWalletTransactionDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    AmountRefunded = table.Column<double>(type: "float", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentWalletTransactionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentWalletTransactionDetails_StudentWalletTransactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "StudentWalletTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentWalletTransactionDetails_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentWalletTransactionDetails_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelScheduleItems_PeriodId",
                table: "ClassLevelScheduleItems",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentWalletTransactionDetails_CreatedBy",
                table: "StudentWalletTransactionDetails",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StudentWalletTransactionDetails_TransactionId",
                table: "StudentWalletTransactionDetails",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentWalletTransactionDetails_UpdatedBy",
                table: "StudentWalletTransactionDetails",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevelScheduleItems_MealSessionDetails_SessionId",
                table: "ClassLevelScheduleItems",
                column: "SessionId",
                principalTable: "MealSessionDetails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevelScheduleItems_MealSessions_PeriodId",
                table: "ClassLevelScheduleItems",
                column: "PeriodId",
                principalTable: "MealSessions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevelScheduleItems_MealSessionDetails_SessionId",
                table: "ClassLevelScheduleItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevelScheduleItems_MealSessions_PeriodId",
                table: "ClassLevelScheduleItems");

            migrationBuilder.DropTable(
                name: "StudentWalletTransactionDetails");

            migrationBuilder.DropIndex(
                name: "IX_ClassLevelScheduleItems_PeriodId",
                table: "ClassLevelScheduleItems");

            migrationBuilder.AddColumn<int>(
                name: "MealPeriodId",
                table: "ClassLevelScheduleItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MealSessionId",
                table: "ClassLevelScheduleItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelScheduleItems_MealPeriodId",
                table: "ClassLevelScheduleItems",
                column: "MealPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelScheduleItems_MealSessionId",
                table: "ClassLevelScheduleItems",
                column: "MealSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevelScheduleItems_MealSessionDetails_MealSessionId",
                table: "ClassLevelScheduleItems",
                column: "MealSessionId",
                principalTable: "MealSessionDetails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevelScheduleItems_MealSessions_MealPeriodId",
                table: "ClassLevelScheduleItems",
                column: "MealPeriodId",
                principalTable: "MealSessions",
                principalColumn: "Id");
        }
    }
}
