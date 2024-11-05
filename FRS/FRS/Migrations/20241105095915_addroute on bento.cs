using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class addrouteonbento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RouteId",
                table: "BentoAssets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BentoAssets_RouteId",
                table: "BentoAssets",
                column: "RouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_BentoAssets_Routes_RouteId",
                table: "BentoAssets",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BentoAssets_Routes_RouteId",
                table: "BentoAssets");

            migrationBuilder.DropIndex(
                name: "IX_BentoAssets_RouteId",
                table: "BentoAssets");

            migrationBuilder.DropColumn(
                name: "RouteId",
                table: "BentoAssets");
        }
    }
}
