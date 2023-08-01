using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class ordercombineddish : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TokenOrderCombinedDish",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    TokenOrderedId = table.Column<int>(nullable: true),
                    CDishLabel = table.Column<string>(nullable: true),
                    CDishCode = table.Column<string>(nullable: true),
                    Qty = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenOrderCombinedDish", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokenOrderCombinedDish_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenOrderCombinedDish_TokenOrdereds_TokenOrderedId",
                        column: x => x.TokenOrderedId,
                        principalTable: "TokenOrdereds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenOrderCombinedDish_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrderCombinedDish_CreatedBy",
                table: "TokenOrderCombinedDish",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrderCombinedDish_TokenOrderedId",
                table: "TokenOrderCombinedDish",
                column: "TokenOrderedId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrderCombinedDish_UpdatedBy",
                table: "TokenOrderCombinedDish",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TokenOrderCombinedDish");
        }
    }
}
