using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addPreviewSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PreviewHeight",
                table: "SignageComponents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreviewWidth",
                table: "SignageComponents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviewHeight",
                table: "SignageComponents");

            migrationBuilder.DropColumn(
                name: "PreviewWidth",
                table: "SignageComponents");
        }
    }
}
