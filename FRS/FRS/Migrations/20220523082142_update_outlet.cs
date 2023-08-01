using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class update_outlet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Outlets_CatererInfos_CatererInfoId",
                table: "Outlets");

            migrationBuilder.DropIndex(
                name: "IX_Outlets_CatererInfoId",
                table: "Outlets");

            migrationBuilder.DropColumn(
                name: "CatererInfoId",
                table: "Outlets");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CatererOutlets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "CatererOutlets");

            migrationBuilder.AddColumn<int>(
                name: "CatererInfoId",
                table: "Outlets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Outlets_CatererInfoId",
                table: "Outlets",
                column: "CatererInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Outlets_CatererInfos_CatererInfoId",
                table: "Outlets",
                column: "CatererInfoId",
                principalTable: "CatererInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
