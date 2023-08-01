using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class add_image_ref_color : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageReferenceColorId",
                table: "LocationImageReferences",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ImageReferenceColors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ColorCode = table.Column<string>(nullable: true),
                    InstitutionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageReferenceColors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageReferenceColors_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImageReferenceColors_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImageReferenceColors_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationImageReferences_ImageReferenceColorId",
                table: "LocationImageReferences",
                column: "ImageReferenceColorId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageReferenceColors_CreatedBy",
                table: "ImageReferenceColors",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ImageReferenceColors_InstitutionId",
                table: "ImageReferenceColors",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageReferenceColors_UpdatedBy",
                table: "ImageReferenceColors",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationImageReferences_ImageReferenceColors_ImageReferenceColorId",
                table: "LocationImageReferences",
                column: "ImageReferenceColorId",
                principalTable: "ImageReferenceColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationImageReferences_ImageReferenceColors_ImageReferenceColorId",
                table: "LocationImageReferences");

            migrationBuilder.DropTable(
                name: "ImageReferenceColors");

            migrationBuilder.DropIndex(
                name: "IX_LocationImageReferences_ImageReferenceColorId",
                table: "LocationImageReferences");

            migrationBuilder.DropColumn(
                name: "ImageReferenceColorId",
                table: "LocationImageReferences");
        }
    }
}
