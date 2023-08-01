using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class outlettostore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "OutletId",
                table: "StoreInfos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreInfos_OutletId",
                table: "StoreInfos",
                column: "OutletId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreInfos_Outlets_OutletId",
                table: "StoreInfos",
                column: "OutletId",
                principalTable: "Outlets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreInfos_Outlets_OutletId",
                table: "StoreInfos");

            migrationBuilder.DropIndex(
                name: "IX_StoreInfos_OutletId",
                table: "StoreInfos");

            migrationBuilder.DropColumn(
                name: "OutletId",
                table: "StoreInfos");
        }
    }
}
