using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addProductionPicture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductionPictureId",
                table: "Dishes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_ProductionPictureId",
                table: "Dishes",
                column: "ProductionPictureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_Files_ProductionPictureId",
                table: "Dishes",
                column: "ProductionPictureId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_Files_ProductionPictureId",
                table: "Dishes");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_ProductionPictureId",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "ProductionPictureId",
                table: "Dishes");
        }
    }
}
