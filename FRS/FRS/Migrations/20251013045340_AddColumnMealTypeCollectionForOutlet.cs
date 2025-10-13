using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnMealTypeCollectionForOutlet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Outlets_MealCollectionType",
                table: "Outlets");

            migrationBuilder.DropColumn(
                name: "MealCollectionType",
                table: "Outlets");
        }
    }
}
