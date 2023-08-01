using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addDirectoryListing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DirectoryListingCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Label = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectoryListingCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectoryListingCategory_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DirectoryListingCategory_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DirectoryListing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Label = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ExtraField = table.Column<string>(nullable: true),
                    IconUrl = table.Column<string>(nullable: true),
                    AvailableForBooking = table.Column<bool>(nullable: false),
                    IsInternal = table.Column<bool>(nullable: false),
                    Aliases = table.Column<string>(nullable: true),
                    Disciplines = table.Column<string>(nullable: true),
                    Contact = table.Column<string>(nullable: true),
                    UnitNumber = table.Column<string>(nullable: true),
                    HighlightIcon = table.Column<string>(nullable: true),
                    HighlightText = table.Column<string>(nullable: true),
                    Pause = table.Column<bool>(nullable: false),
                    PauseTime = table.Column<int>(nullable: true),
                    MapDisplayName = table.Column<string>(nullable: true),
                    DirectoryListingCategoryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectoryListing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectoryListing_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DirectoryListing_DirectoryListingCategory_DirectoryListingCategoryId",
                        column: x => x.DirectoryListingCategoryId,
                        principalTable: "DirectoryListingCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DirectoryListing_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DirectoryListing_CreatedBy",
                table: "DirectoryListing",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DirectoryListing_DirectoryListingCategoryId",
                table: "DirectoryListing",
                column: "DirectoryListingCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectoryListing_UpdatedBy",
                table: "DirectoryListing",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DirectoryListingCategory_CreatedBy",
                table: "DirectoryListingCategory",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DirectoryListingCategory_UpdatedBy",
                table: "DirectoryListingCategory",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DirectoryListing");

            migrationBuilder.DropTable(
                name: "DirectoryListingCategory");
        }
    }
}
