using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class update_orderportal_new_cols : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "OrderPortalBanners",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "OrderPortalBanners",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "OrderPortalBanners",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageFilePath",
                table: "OrderPortalBanners",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "OrderPortalBanners");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "OrderPortalBanners");

            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "OrderPortalBanners");

            migrationBuilder.DropColumn(
                name: "ImageFilePath",
                table: "OrderPortalBanners");
        }
    }
}
