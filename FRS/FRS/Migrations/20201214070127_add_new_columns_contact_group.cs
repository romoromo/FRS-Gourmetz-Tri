using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class add_new_columns_contact_group : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HomeNo",
                table: "ContactGroupMembers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "ContactGroupMembers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomeNo",
                table: "ContactGroupMembers");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "ContactGroupMembers");
        }
    }
}
