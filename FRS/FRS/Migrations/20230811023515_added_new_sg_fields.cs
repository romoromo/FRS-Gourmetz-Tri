using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_new_sg_fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "StudentGroups",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OutletTermId",
                table: "StudentGroups",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OutletTerms",
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
                    OutletId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletTerms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutletTerms_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletTerms_Outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletTerms_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroups_OutletTermId",
                table: "StudentGroups",
                column: "OutletTermId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletTerms_CreatedBy",
                table: "OutletTerms",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutletTerms_OutletId",
                table: "OutletTerms",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletTerms_UpdatedBy",
                table: "OutletTerms",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentGroups_OutletTerms_OutletTermId",
                table: "StudentGroups",
                column: "OutletTermId",
                principalTable: "OutletTerms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentGroups_OutletTerms_OutletTermId",
                table: "StudentGroups");

            migrationBuilder.DropTable(
                name: "OutletTerms");

            migrationBuilder.DropIndex(
                name: "IX_StudentGroups_OutletTermId",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "OutletTermId",
                table: "StudentGroups");
        }
    }
}
