using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_caterer_to_associatetables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CatererId",
                table: "OutletProfiles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CatererId",
                table: "MealTypes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CatererId",
                table: "DishTypes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutletProfiles_CatererId",
                table: "OutletProfiles",
                column: "CatererId");

            migrationBuilder.CreateIndex(
                name: "IX_MealTypes_CatererId",
                table: "MealTypes",
                column: "CatererId");

            migrationBuilder.CreateIndex(
                name: "IX_DishTypes_CatererId",
                table: "DishTypes",
                column: "CatererId");

            migrationBuilder.AddForeignKey(
                name: "FK_DishTypes_CatererInfos_CatererId",
                table: "DishTypes",
                column: "CatererId",
                principalTable: "CatererInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MealTypes_CatererInfos_CatererId",
                table: "MealTypes",
                column: "CatererId",
                principalTable: "CatererInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OutletProfiles_CatererInfos_CatererId",
                table: "OutletProfiles",
                column: "CatererId",
                principalTable: "CatererInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishTypes_CatererInfos_CatererId",
                table: "DishTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_MealTypes_CatererInfos_CatererId",
                table: "MealTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_OutletProfiles_CatererInfos_CatererId",
                table: "OutletProfiles");

            migrationBuilder.DropIndex(
                name: "IX_OutletProfiles_CatererId",
                table: "OutletProfiles");

            migrationBuilder.DropIndex(
                name: "IX_MealTypes_CatererId",
                table: "MealTypes");

            migrationBuilder.DropIndex(
                name: "IX_DishTypes_CatererId",
                table: "DishTypes");

            migrationBuilder.DropColumn(
                name: "CatererId",
                table: "OutletProfiles");

            migrationBuilder.DropColumn(
                name: "CatererId",
                table: "MealTypes");

            migrationBuilder.DropColumn(
                name: "CatererId",
                table: "DishTypes");
        }
    }
}
