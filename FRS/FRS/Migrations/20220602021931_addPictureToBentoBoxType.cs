using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addPictureToBentoBoxType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CalendarDays",
                table: "StoreInfos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "StoreInfos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CutoffTime",
                table: "StoreInfos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "picture",
                table: "BentoBoxTypes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalendarDays",
                table: "StoreInfos");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "StoreInfos");

            migrationBuilder.DropColumn(
                name: "CutoffTime",
                table: "StoreInfos");

            migrationBuilder.DropColumn(
                name: "picture",
                table: "BentoBoxTypes");
        }
    }
}
