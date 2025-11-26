using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddCatererAsset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CatererAssetTypeId",
                table: "CartonAssets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CatererAssets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    assetQRCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CatererAssetTypeId = table.Column<int>(type: "int", nullable: true),
                    FileId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatererAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatererAssets_CatererAssetTypes_CatererAssetTypeId",
                        column: x => x.CatererAssetTypeId,
                        principalTable: "CatererAssetTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CatererAssets_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CatererAssets_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CatererAssets_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartonAssets_CatererAssetTypeId",
                table: "CartonAssets",
                column: "CatererAssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CatererAssets_assetQRCode_Description_CatererAssetTypeId",
                table: "CatererAssets",
                columns: new[] { "assetQRCode", "Description", "CatererAssetTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_CatererAssets_CatererAssetTypeId",
                table: "CatererAssets",
                column: "CatererAssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CatererAssets_CreatedBy",
                table: "CatererAssets",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CatererAssets_FileId",
                table: "CatererAssets",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_CatererAssets_UpdatedBy",
                table: "CatererAssets",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_CartonAssets_CatererAssetTypes_CatererAssetTypeId",
                table: "CartonAssets",
                column: "CatererAssetTypeId",
                principalTable: "CatererAssetTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartonAssets_CatererAssetTypes_CatererAssetTypeId",
                table: "CartonAssets");

            migrationBuilder.DropTable(
                name: "CatererAssets");

            migrationBuilder.DropIndex(
                name: "IX_CartonAssets_CatererAssetTypeId",
                table: "CartonAssets");

            migrationBuilder.DropColumn(
                name: "CatererAssetTypeId",
                table: "CartonAssets");
        }
    }
}
