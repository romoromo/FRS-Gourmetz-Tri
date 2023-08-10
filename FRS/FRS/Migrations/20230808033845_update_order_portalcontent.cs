using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class update_order_portalcontent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OutletId",
                table: "OrderPortalBanners",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutletId",
                table: "OrderPortalBanners");
        }
    }
}
