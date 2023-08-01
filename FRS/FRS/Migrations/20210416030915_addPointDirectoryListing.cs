using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addPointDirectoryListing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PointDirectoryListing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    PointId = table.Column<int>(nullable: true),
                    DirectoryListingId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointDirectoryListing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointDirectoryListing_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PointDirectoryListing_DirectoryListing_DirectoryListingId",
                        column: x => x.DirectoryListingId,
                        principalTable: "DirectoryListing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PointDirectoryListing_Point_PointId",
                        column: x => x.PointId,
                        principalTable: "Point",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PointDirectoryListing_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointDirectoryListing_CreatedBy",
                table: "PointDirectoryListing",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PointDirectoryListing_DirectoryListingId",
                table: "PointDirectoryListing",
                column: "DirectoryListingId");

            migrationBuilder.CreateIndex(
                name: "IX_PointDirectoryListing_PointId",
                table: "PointDirectoryListing",
                column: "PointId");

            migrationBuilder.CreateIndex(
                name: "IX_PointDirectoryListing_UpdatedBy",
                table: "PointDirectoryListing",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointDirectoryListing");
        }
    }
}
