using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class update_vouchers_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Vouchers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OutletProfileId",
                table: "Vouchers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "OutletProfileId",
                table: "Vouchers");
        }
    }
}
