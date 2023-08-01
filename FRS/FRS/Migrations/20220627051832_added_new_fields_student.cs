using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_new_fields_student : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreInventoryDetails_StoreInfos_StoreInfoId",
                table: "StoreInventoryDetails");

            migrationBuilder.AddColumn<float>(
                name: "TargetWeeklyCalIntake",
                table: "Students",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<int>(
                name: "StoreInfoId",
                table: "StoreInventoryDetails",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<float>(
                name: "Protein",
                table: "Dishes",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Sugar",
                table: "Dishes",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "TotalCarb",
                table: "Dishes",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "TotalFat",
                table: "Dishes",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreInventoryDetails_StoreInfos_StoreInfoId",
                table: "StoreInventoryDetails",
                column: "StoreInfoId",
                principalTable: "StoreInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreInventoryDetails_StoreInfos_StoreInfoId",
                table: "StoreInventoryDetails");

            migrationBuilder.DropColumn(
                name: "TargetWeeklyCalIntake",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Protein",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "Sugar",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "TotalCarb",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "TotalFat",
                table: "Dishes");

            migrationBuilder.AlterColumn<int>(
                name: "StoreInfoId",
                table: "StoreInventoryDetails",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreInventoryDetails_StoreInfos_StoreInfoId",
                table: "StoreInventoryDetails",
                column: "StoreInfoId",
                principalTable: "StoreInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
