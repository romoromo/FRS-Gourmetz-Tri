using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addMediaDescriptionTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ImageFiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "ImageFiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ImageFiles");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "ImageFiles");
        }
    }
}
