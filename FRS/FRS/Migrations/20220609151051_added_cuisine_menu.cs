using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_cuisine_menu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CuisineId",
                table: "Menus",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Menus_CuisineId",
                table: "Menus",
                column: "CuisineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_Cuisines_CuisineId",
                table: "Menus",
                column: "CuisineId",
                principalTable: "Cuisines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Cuisines_CuisineId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Menus_CuisineId",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "CuisineId",
                table: "Menus");
        }
    }
}
