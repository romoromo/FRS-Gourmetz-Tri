using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_classbatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClassBatchId",
                table: "Classes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClassBatches",
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
                    Year = table.Column<int>(nullable: false),
                    InstitutionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassBatches_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassBatches_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassBatches_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classes_ClassBatchId",
                table: "Classes",
                column: "ClassBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassBatches_CreatedBy",
                table: "ClassBatches",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ClassBatches_InstitutionId",
                table: "ClassBatches",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassBatches_UpdatedBy",
                table: "ClassBatches",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_ClassBatches_ClassBatchId",
                table: "Classes",
                column: "ClassBatchId",
                principalTable: "ClassBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_ClassBatches_ClassBatchId",
                table: "Classes");

            migrationBuilder.DropTable(
                name: "ClassBatches");

            migrationBuilder.DropIndex(
                name: "IX_Classes_ClassBatchId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ClassBatchId",
                table: "Classes");
        }
    }
}
