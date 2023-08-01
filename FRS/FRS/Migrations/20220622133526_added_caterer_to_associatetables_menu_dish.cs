using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_caterer_to_associatetables_menu_dish : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CatererId",
                table: "Menus",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CatererId",
                table: "Dishes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Menus_CatererId",
                table: "Menus",
                column: "CatererId");

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_CatererId",
                table: "Dishes",
                column: "CatererId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_CatererInfos_CatererId",
                table: "Dishes",
                column: "CatererId",
                principalTable: "CatererInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_CatererInfos_CatererId",
                table: "Menus",
                column: "CatererId",
                principalTable: "CatererInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_CatererInfos_CatererId",
                table: "Dishes");

            migrationBuilder.DropForeignKey(
                name: "FK_Menus_CatererInfos_CatererId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Menus_CatererId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_CatererId",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "CatererId",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "CatererId",
                table: "Dishes");
        }
    }
}
