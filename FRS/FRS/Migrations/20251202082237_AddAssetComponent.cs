using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetComponent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatererAssetId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FileId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetComponents_CatererAssets_CatererAssetId",
                        column: x => x.CatererAssetId,
                        principalTable: "CatererAssets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetComponents_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetComponents_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetComponents_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetComponents_CatererAssetId",
                table: "AssetComponents",
                column: "CatererAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetComponents_CreatedBy",
                table: "AssetComponents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AssetComponents_FileId",
                table: "AssetComponents",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetComponents_Remarks_Description_CatererAssetId",
                table: "AssetComponents",
                columns: new[] { "Remarks", "Description", "CatererAssetId" });

            migrationBuilder.CreateIndex(
                name: "IX_AssetComponents_UpdatedBy",
                table: "AssetComponents",
                column: "UpdatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetComponents");
        }
    }
}
