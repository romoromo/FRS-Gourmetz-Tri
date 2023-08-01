using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addPublicationHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApprovedBy",
                table: "SignagePublications",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "SignagePublications",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Rejected",
                table: "SignagePublications",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "SignagePublications",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SignagePublicationHistorys",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Approved = table.Column<bool>(nullable: false),
                    Rejected = table.Column<bool>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    ApprovedBy = table.Column<int>(nullable: true),
                    ApprovedDate = table.Column<DateTime>(nullable: false),
                    PublicationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignagePublicationHistorys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignagePublicationHistorys_User_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignagePublicationHistorys_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignagePublicationHistorys_SignagePublications_PublicationId",
                        column: x => x.PublicationId,
                        principalTable: "SignagePublications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignagePublicationHistorys_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SignagePublications_ApprovedBy",
                table: "SignagePublications",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SignagePublicationHistorys_ApprovedBy",
                table: "SignagePublicationHistorys",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SignagePublicationHistorys_CreatedBy",
                table: "SignagePublicationHistorys",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SignagePublicationHistorys_PublicationId",
                table: "SignagePublicationHistorys",
                column: "PublicationId");

            migrationBuilder.CreateIndex(
                name: "IX_SignagePublicationHistorys_UpdatedBy",
                table: "SignagePublicationHistorys",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_SignagePublications_User_ApprovedBy",
                table: "SignagePublications",
                column: "ApprovedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SignagePublications_User_ApprovedBy",
                table: "SignagePublications");

            migrationBuilder.DropTable(
                name: "SignagePublicationHistorys");

            migrationBuilder.DropIndex(
                name: "IX_SignagePublications_ApprovedBy",
                table: "SignagePublications");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "SignagePublications");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "SignagePublications");

            migrationBuilder.DropColumn(
                name: "Rejected",
                table: "SignagePublications");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "SignagePublications");
        }
    }
}
