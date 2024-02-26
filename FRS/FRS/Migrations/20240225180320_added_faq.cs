using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_faq : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FaqSubjects",
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
                    Description = table.Column<string>(nullable: true),
                    InstitutionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaqSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaqSubjects_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FaqSubjects_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FaqSubjects_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FaqDetails",
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
                    Description = table.Column<string>(nullable: true),
                    InstitutionId = table.Column<int>(nullable: true),
                    FaqSubjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaqDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaqDetails_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FaqDetails_FaqSubjects_FaqSubjectId",
                        column: x => x.FaqSubjectId,
                        principalTable: "FaqSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaqDetails_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FaqDetails_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FaqDetails_CreatedBy",
                table: "FaqDetails",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FaqDetails_FaqSubjectId",
                table: "FaqDetails",
                column: "FaqSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_FaqDetails_InstitutionId",
                table: "FaqDetails",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_FaqDetails_UpdatedBy",
                table: "FaqDetails",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FaqSubjects_CreatedBy",
                table: "FaqSubjects",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FaqSubjects_InstitutionId",
                table: "FaqSubjects",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_FaqSubjects_UpdatedBy",
                table: "FaqSubjects",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FaqDetails");

            migrationBuilder.DropTable(
                name: "FaqSubjects");
        }
    }
}
