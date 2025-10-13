using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class fileiconcuisine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "Cuisines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cuisines_FileId",
                table: "Cuisines",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cuisines_Files_FileId",
                table: "Cuisines",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cuisines_Files_FileId",
                table: "Cuisines");

            migrationBuilder.DropIndex(
                name: "IX_Cuisines_FileId",
                table: "Cuisines");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "Cuisines");
        }
    }
}
