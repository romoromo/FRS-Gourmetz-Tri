using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class fixperiod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TokenOrders_Students_StudentId",
                table: "TokenOrders");

            migrationBuilder.DropIndex(
                name: "IX_TokenOrders_StudentId",
                table: "TokenOrders");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "TokenOrders");

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrders_ProfileId",
                table: "TokenOrders",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_TokenOrders_Students_ProfileId",
                table: "TokenOrders",
                column: "ProfileId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TokenOrders_Students_ProfileId",
                table: "TokenOrders");

            migrationBuilder.DropIndex(
                name: "IX_TokenOrders_ProfileId",
                table: "TokenOrders");

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "TokenOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrders_StudentId",
                table: "TokenOrders",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TokenOrders_Students_StudentId",
                table: "TokenOrders",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
