using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AlterSortingArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SortingAreas_Code_Description_CatererId_RouteId",
                table: "SortingAreas");

            migrationBuilder.DropColumn(
                name: "CatererId",
                table: "SortingAreas");

            migrationBuilder.CreateIndex(
                name: "IX_SortingAreas_Code_Description_CatererInfoId_RouteId",
                table: "SortingAreas",
                columns: new[] { "Code", "Description", "CatererInfoId", "RouteId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SortingAreas_Code_Description_CatererInfoId_RouteId",
                table: "SortingAreas");

            migrationBuilder.AddColumn<int>(
                name: "CatererId",
                table: "SortingAreas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SortingAreas_Code_Description_CatererId_RouteId",
                table: "SortingAreas",
                columns: new[] { "Code", "Description", "CatererId", "RouteId" });
        }
    }
}
