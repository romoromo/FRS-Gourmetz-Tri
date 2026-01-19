using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AlterCuisineAddKitchenLabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KitchenLabel",
                table: "Cuisines",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KitchenLabel",
                table: "Cuisines");
        }
    }
}
