using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class tokenordermealsession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TokenOrders_MealPeriods_PeriodId",
                table: "TokenOrders");

            migrationBuilder.AlterColumn<int>(
                name: "PeriodId",
                table: "TokenOrders",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "MealSessionDetailId",
                table: "TokenOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrders_MealSessionDetailId",
                table: "TokenOrders",
                column: "MealSessionDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_TokenOrders_MealSessionDetails_MealSessionDetailId",
                table: "TokenOrders",
                column: "MealSessionDetailId",
                principalTable: "MealSessionDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TokenOrders_MealPeriods_PeriodId",
                table: "TokenOrders",
                column: "PeriodId",
                principalTable: "MealPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TokenOrders_MealSessionDetails_MealSessionDetailId",
                table: "TokenOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_TokenOrders_MealPeriods_PeriodId",
                table: "TokenOrders");

            migrationBuilder.DropIndex(
                name: "IX_TokenOrders_MealSessionDetailId",
                table: "TokenOrders");

            migrationBuilder.DropColumn(
                name: "MealSessionDetailId",
                table: "TokenOrders");

            migrationBuilder.AlterColumn<int>(
                name: "PeriodId",
                table: "TokenOrders",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TokenOrders_MealPeriods_PeriodId",
                table: "TokenOrders",
                column: "PeriodId",
                principalTable: "MealPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
