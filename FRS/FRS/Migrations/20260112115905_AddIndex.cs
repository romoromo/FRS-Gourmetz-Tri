using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Students_ClassLevelId",
                table: "Students");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ClassLevelId_Id",
                table: "Students",
                columns: new[] { "ClassLevelId", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Students_ClassLevelId_Id",
                table: "Students");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ClassLevelId",
                table: "Students",
                column: "ClassLevelId");
        }
    }
}
