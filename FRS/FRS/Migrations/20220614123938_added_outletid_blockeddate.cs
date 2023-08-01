using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_outletid_blockeddate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OutletId",
                table: "OutletBlockedDates",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutletBlockedDates_OutletId",
                table: "OutletBlockedDates",
                column: "OutletId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutletBlockedDates_Outlets_OutletId",
                table: "OutletBlockedDates",
                column: "OutletId",
                principalTable: "Outlets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutletBlockedDates_Outlets_OutletId",
                table: "OutletBlockedDates");

            migrationBuilder.DropIndex(
                name: "IX_OutletBlockedDates_OutletId",
                table: "OutletBlockedDates");

            migrationBuilder.DropColumn(
                name: "OutletId",
                table: "OutletBlockedDates");
        }
    }
}
