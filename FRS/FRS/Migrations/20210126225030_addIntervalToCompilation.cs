using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addIntervalToCompilation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "interval",
                table: "SignageScheduleCompilations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "interval",
                table: "SignageScheduleCompilations");
        }
    }
}
