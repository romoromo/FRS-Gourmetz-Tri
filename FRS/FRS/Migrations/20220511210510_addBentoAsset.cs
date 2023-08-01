using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addBentoAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BentoAssets",
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
                    InstitutionId = table.Column<int>(nullable: true),
                    BentoBoxTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BentoAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BentoAssets_BentoBoxTypes_BentoBoxTypeId",
                        column: x => x.BentoBoxTypeId,
                        principalTable: "BentoBoxTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BentoAssets_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BentoAssets_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BentoAssets_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BentoAssets_BentoBoxTypeId",
                table: "BentoAssets",
                column: "BentoBoxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BentoAssets_CreatedBy",
                table: "BentoAssets",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BentoAssets_InstitutionId",
                table: "BentoAssets",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_BentoAssets_UpdatedBy",
                table: "BentoAssets",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BentoAssets");
        }
    }
}
