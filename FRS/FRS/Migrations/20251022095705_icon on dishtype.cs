using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class iconondishtype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "DishTypes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DishTypes_FileId",
                table: "DishTypes",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_DishTypes_Files_FileId",
                table: "DishTypes",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishTypes_Files_FileId",
                table: "DishTypes");

            migrationBuilder.DropIndex(
                name: "IX_DishTypes_FileId",
                table: "DishTypes");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "DishTypes");
        }
    }
}
