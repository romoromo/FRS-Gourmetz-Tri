using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_studentid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "CancelOrderRequests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CancelOrderRequests_StudentId",
                table: "CancelOrderRequests",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CancelOrderRequests_Students_StudentId",
                table: "CancelOrderRequests",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CancelOrderRequests_Students_StudentId",
                table: "CancelOrderRequests");

            migrationBuilder.DropIndex(
                name: "IX_CancelOrderRequests_StudentId",
                table: "CancelOrderRequests");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "CancelOrderRequests");
        }
    }
}
