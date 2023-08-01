using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_voucher_meal_periods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoucherMealPeriod_User_CreatedBy",
                table: "VoucherMealPeriod");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherMealPeriod_MealPeriods_MealPeriodId",
                table: "VoucherMealPeriod");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherMealPeriod_User_UpdatedBy",
                table: "VoucherMealPeriod");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherMealPeriod_Vouchers_VoucherId",
                table: "VoucherMealPeriod");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VoucherMealPeriod",
                table: "VoucherMealPeriod");

            migrationBuilder.RenameTable(
                name: "VoucherMealPeriod",
                newName: "VoucherMealPeriods");

            migrationBuilder.RenameIndex(
                name: "IX_VoucherMealPeriod_VoucherId",
                table: "VoucherMealPeriods",
                newName: "IX_VoucherMealPeriods_VoucherId");

            migrationBuilder.RenameIndex(
                name: "IX_VoucherMealPeriod_UpdatedBy",
                table: "VoucherMealPeriods",
                newName: "IX_VoucherMealPeriods_UpdatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_VoucherMealPeriod_MealPeriodId",
                table: "VoucherMealPeriods",
                newName: "IX_VoucherMealPeriods_MealPeriodId");

            migrationBuilder.RenameIndex(
                name: "IX_VoucherMealPeriod_CreatedBy",
                table: "VoucherMealPeriods",
                newName: "IX_VoucherMealPeriods_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoucherMealPeriods",
                table: "VoucherMealPeriods",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherMealPeriods_User_CreatedBy",
                table: "VoucherMealPeriods",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherMealPeriods_MealPeriods_MealPeriodId",
                table: "VoucherMealPeriods",
                column: "MealPeriodId",
                principalTable: "MealPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherMealPeriods_User_UpdatedBy",
                table: "VoucherMealPeriods",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherMealPeriods_Vouchers_VoucherId",
                table: "VoucherMealPeriods",
                column: "VoucherId",
                principalTable: "Vouchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoucherMealPeriods_User_CreatedBy",
                table: "VoucherMealPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherMealPeriods_MealPeriods_MealPeriodId",
                table: "VoucherMealPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherMealPeriods_User_UpdatedBy",
                table: "VoucherMealPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherMealPeriods_Vouchers_VoucherId",
                table: "VoucherMealPeriods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VoucherMealPeriods",
                table: "VoucherMealPeriods");

            migrationBuilder.RenameTable(
                name: "VoucherMealPeriods",
                newName: "VoucherMealPeriod");

            migrationBuilder.RenameIndex(
                name: "IX_VoucherMealPeriods_VoucherId",
                table: "VoucherMealPeriod",
                newName: "IX_VoucherMealPeriod_VoucherId");

            migrationBuilder.RenameIndex(
                name: "IX_VoucherMealPeriods_UpdatedBy",
                table: "VoucherMealPeriod",
                newName: "IX_VoucherMealPeriod_UpdatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_VoucherMealPeriods_MealPeriodId",
                table: "VoucherMealPeriod",
                newName: "IX_VoucherMealPeriod_MealPeriodId");

            migrationBuilder.RenameIndex(
                name: "IX_VoucherMealPeriods_CreatedBy",
                table: "VoucherMealPeriod",
                newName: "IX_VoucherMealPeriod_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoucherMealPeriod",
                table: "VoucherMealPeriod",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherMealPeriod_User_CreatedBy",
                table: "VoucherMealPeriod",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherMealPeriod_MealPeriods_MealPeriodId",
                table: "VoucherMealPeriod",
                column: "MealPeriodId",
                principalTable: "MealPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherMealPeriod_User_UpdatedBy",
                table: "VoucherMealPeriod",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherMealPeriod_Vouchers_VoucherId",
                table: "VoucherMealPeriod",
                column: "VoucherId",
                principalTable: "Vouchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
