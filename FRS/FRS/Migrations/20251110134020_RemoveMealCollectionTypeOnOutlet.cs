using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMealCollectionTypeOnOutlet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Outlets_MealCollectionType",
                table: "Outlets");

            migrationBuilder.DropColumn(
                name: "MealCollectionType",
                table: "Outlets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MealCollectionType",
                table: "Outlets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Outlets_MealCollectionType",
                table: "Outlets",
                column: "MealCollectionType");
        }
    }
}
