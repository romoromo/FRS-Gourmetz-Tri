using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addStoreToDish : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SapCode",
                table: "Dishes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StoreInfoId",
                table: "Dishes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_StoreInfoId",
                table: "Dishes",
                column: "StoreInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_StoreInfos_StoreInfoId",
                table: "Dishes",
                column: "StoreInfoId",
                principalTable: "StoreInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_StoreInfos_StoreInfoId",
                table: "Dishes");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_StoreInfoId",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "SapCode",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "StoreInfoId",
                table: "Dishes");
        }
    }
}
