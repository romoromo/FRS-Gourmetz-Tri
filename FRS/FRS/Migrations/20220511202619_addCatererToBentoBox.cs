using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addCatererToBentoBox : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CatererInfoId",
                table: "BentoBoxTypes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BentoBoxTypes_CatererInfoId",
                table: "BentoBoxTypes",
                column: "CatererInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_BentoBoxTypes_CatererInfos_CatererInfoId",
                table: "BentoBoxTypes",
                column: "CatererInfoId",
                principalTable: "CatererInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BentoBoxTypes_CatererInfos_CatererInfoId",
                table: "BentoBoxTypes");

            migrationBuilder.DropIndex(
                name: "IX_BentoBoxTypes_CatererInfoId",
                table: "BentoBoxTypes");

            migrationBuilder.DropColumn(
                name: "CatererInfoId",
                table: "BentoBoxTypes");
        }
    }
}
