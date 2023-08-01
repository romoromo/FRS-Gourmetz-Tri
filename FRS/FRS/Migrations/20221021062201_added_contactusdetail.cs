using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_contactusdetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactUsDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    InstitutionId = table.Column<int>(nullable: true),
                    ContactUsSubjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactUsDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactUsDetails_ContactUsSubjects_ContactUsSubjectId",
                        column: x => x.ContactUsSubjectId,
                        principalTable: "ContactUsSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContactUsDetails_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContactUsDetails_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContactUsDetails_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactUsDetails_ContactUsSubjectId",
                table: "ContactUsDetails",
                column: "ContactUsSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactUsDetails_CreatedBy",
                table: "ContactUsDetails",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContactUsDetails_InstitutionId",
                table: "ContactUsDetails",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactUsDetails_UpdatedBy",
                table: "ContactUsDetails",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactUsDetails");
        }
    }
}
