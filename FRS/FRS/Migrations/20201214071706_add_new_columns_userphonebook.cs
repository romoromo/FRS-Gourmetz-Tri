using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class add_new_columns_userphonebook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HomeNo",
                table: "UserPhonebooks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "UserPhonebooks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomeNo",
                table: "UserPhonebooks");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "UserPhonebooks");
        }
    }
}
