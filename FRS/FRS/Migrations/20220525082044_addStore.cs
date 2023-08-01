using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addStore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "CatererInfos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "CatererInfos");
        }
    }
}
