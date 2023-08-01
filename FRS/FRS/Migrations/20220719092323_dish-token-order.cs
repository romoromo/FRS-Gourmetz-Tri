using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class dishtokenorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TokenAltDishes",
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
                    DishId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenAltDishes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokenAltDishes_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenAltDishes_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenAltDishes_TokenOrdereds_TokenOrderedId",
                        column: x => x.TokenOrderedId,
                        principalTable: "TokenOrdereds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenAltDishes_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TokenOrderDishes",
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
                    DishId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenOrderDishes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokenOrderDishes_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenOrderDishes_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenOrderDishes_TokenOrdereds_TokenOrderedId",
                        column: x => x.TokenOrderedId,
                        principalTable: "TokenOrdereds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenOrderDishes_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TokenAltDishes_CreatedBy",
                table: "TokenAltDishes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TokenAltDishes_DishId",
                table: "TokenAltDishes",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenAltDishes_TokenOrderedId",
                table: "TokenAltDishes",
                column: "TokenOrderedId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenAltDishes_UpdatedBy",
                table: "TokenAltDishes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrderDishes_CreatedBy",
                table: "TokenOrderDishes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrderDishes_DishId",
                table: "TokenOrderDishes",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrderDishes_TokenOrderedId",
                table: "TokenOrderDishes",
                column: "TokenOrderedId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrderDishes_UpdatedBy",
                table: "TokenOrderDishes",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TokenAltDishes");

            migrationBuilder.DropTable(
                name: "TokenOrderDishes");
        }
    }
}
