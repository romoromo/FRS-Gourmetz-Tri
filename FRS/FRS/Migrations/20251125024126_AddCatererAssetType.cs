using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddCatererAssetType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatererAssetTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FileId = table.Column<int>(type: "int", nullable: true),
                    CatererInfoId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatererAssetTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatererAssetTypes_CatererInfos_CatererInfoId",
                        column: x => x.CatererInfoId,
                        principalTable: "CatererInfos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CatererAssetTypes_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CatererAssetTypes_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CatererAssetTypes_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatererAssetTypes_CatererInfoId",
                table: "CatererAssetTypes",
                column: "CatererInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_CatererAssetTypes_Code_Description_CatererInfoId",
                table: "CatererAssetTypes",
                columns: new[] { "Code", "Description", "CatererInfoId" });

            migrationBuilder.CreateIndex(
                name: "IX_CatererAssetTypes_CreatedBy",
                table: "CatererAssetTypes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CatererAssetTypes_FileId",
                table: "CatererAssetTypes",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_CatererAssetTypes_UpdatedBy",
                table: "CatererAssetTypes",
                column: "UpdatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatererAssetTypes");
        }
    }
}
