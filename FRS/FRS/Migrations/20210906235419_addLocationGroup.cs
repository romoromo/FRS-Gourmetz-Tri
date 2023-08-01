using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addLocationGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocationGroup",
                table: "Locations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationGroup",
                table: "Locations");
        }
    }
}
