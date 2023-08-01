using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addCodeToLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code0",
                table: "Line",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code1",
                table: "Line",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code0",
                table: "Line");

            migrationBuilder.DropColumn(
                name: "Code1",
                table: "Line");
        }
    }
}
