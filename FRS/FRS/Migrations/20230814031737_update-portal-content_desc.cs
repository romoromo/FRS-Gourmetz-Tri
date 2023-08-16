using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class updateportalcontent_desc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "OrderPortalContents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "OrderPortalContents");
        }
    }
}
