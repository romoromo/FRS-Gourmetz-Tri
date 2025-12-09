using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddSortingAreas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SortingAreas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CatererId = table.Column<int>(type: "int", nullable: true),
                    CatererInfoId = table.Column<int>(type: "int", nullable: true),
                    RouteId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SortingAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SortingAreas_CatererInfos_CatererInfoId",
                        column: x => x.CatererInfoId,
                        principalTable: "CatererInfos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SortingAreas_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SortingAreas_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SortingAreas_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SortingAreas_CatererInfoId",
                table: "SortingAreas",
                column: "CatererInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_SortingAreas_Code_Description_CatererId_RouteId",
                table: "SortingAreas",
                columns: new[] { "Code", "Description", "CatererId", "RouteId" });

            migrationBuilder.CreateIndex(
                name: "IX_SortingAreas_CreatedBy",
                table: "SortingAreas",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SortingAreas_RouteId",
                table: "SortingAreas",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_SortingAreas_UpdatedBy",
                table: "SortingAreas",
                column: "UpdatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SortingAreas");
        }
    }
}
