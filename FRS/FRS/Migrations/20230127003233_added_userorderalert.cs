using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_userorderalert : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserOrderAlert",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    TokenOrderId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    IsAbandonedCart1Sent = table.Column<bool>(nullable: false),
                    IsAbandonedCart2Sent = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrderAlert", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOrderAlert_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserOrderAlert_TokenOrders_TokenOrderId",
                        column: x => x.TokenOrderId,
                        principalTable: "TokenOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserOrderAlert_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserOrderAlert_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserOrderAlert_CreatedBy",
                table: "UserOrderAlert",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrderAlert_TokenOrderId",
                table: "UserOrderAlert",
                column: "TokenOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrderAlert_UpdatedBy",
                table: "UserOrderAlert",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrderAlert_UserId",
                table: "UserOrderAlert",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserOrderAlert");
        }
    }
}
