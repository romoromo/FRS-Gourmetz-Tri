using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class routeidonmealallocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RouteId",
                table: "MealAllocations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealAllocations_RouteId",
                table: "MealAllocations",
                column: "RouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_MealAllocations_Routes_RouteId",
                table: "MealAllocations",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealAllocations_Routes_RouteId",
                table: "MealAllocations");

            migrationBuilder.DropIndex(
                name: "IX_MealAllocations_RouteId",
                table: "MealAllocations");

            migrationBuilder.DropColumn(
                name: "RouteId",
                table: "MealAllocations");
        }
    }
}
