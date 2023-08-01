using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class store_to_bento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ToStoreInfoId",
                table: "BentoAssets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ToStoreInfoId",
                table: "BentoAssets");
        }
    }
}
