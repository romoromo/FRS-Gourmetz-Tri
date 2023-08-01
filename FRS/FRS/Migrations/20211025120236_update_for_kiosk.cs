using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class update_for_kiosk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "anchor_h",
                table: "KioskSettings",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "anchor_v",
                table: "KioskSettings",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "baloon_height",
                table: "KioskSettings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "baloon_image",
                table: "KioskSettings",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "baloon_width",
                table: "KioskSettings",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "dir_font_size",
                table: "KioskSettings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "dir_font_type2",
                table: "KioskSettings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "DirectoryListingCategory",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "anchor_h",
                table: "KioskSettings");

            migrationBuilder.DropColumn(
                name: "anchor_v",
                table: "KioskSettings");

            migrationBuilder.DropColumn(
                name: "baloon_height",
                table: "KioskSettings");

            migrationBuilder.DropColumn(
                name: "baloon_image",
                table: "KioskSettings");

            migrationBuilder.DropColumn(
                name: "baloon_width",
                table: "KioskSettings");

            migrationBuilder.DropColumn(
                name: "dir_font_size",
                table: "KioskSettings");

            migrationBuilder.DropColumn(
                name: "dir_font_type2",
                table: "KioskSettings");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "DirectoryListingCategory");
        }
    }
}
