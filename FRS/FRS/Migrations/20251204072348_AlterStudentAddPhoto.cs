using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AlterStudentAddPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PhotoId",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_PhotoId",
                table: "Students",
                column: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Files_PhotoId",
                table: "Students",
                column: "PhotoId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Files_PhotoId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_PhotoId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Students");
        }
    }
}
