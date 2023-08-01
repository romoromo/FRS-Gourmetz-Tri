using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class packinglist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PackingAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    PackingDate = table.Column<DateTime>(nullable: true),
                    RouteId = table.Column<int>(nullable: true),
                    OutletId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackingAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackingAllocations_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PackingAllocations_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PackingAllocations_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PackingAllocations_Outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DishAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    PackingId = table.Column<int>(nullable: true),
                    DishId = table.Column<int>(nullable: true),
                    Qty = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishAllocations_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishAllocations_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishAllocations_PackingAllocations_PackingId",
                        column: x => x.PackingId,
                        principalTable: "PackingAllocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishAllocations_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DishAllocations_CreatedBy",
                table: "DishAllocations",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DishAllocations_DishId",
                table: "DishAllocations",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_DishAllocations_PackingId",
                table: "DishAllocations",
                column: "PackingId");

            migrationBuilder.CreateIndex(
                name: "IX_DishAllocations_UpdatedBy",
                table: "DishAllocations",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PackingAllocations_CreatedBy",
                table: "PackingAllocations",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PackingAllocations_RouteId",
                table: "PackingAllocations",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_PackingAllocations_UpdatedBy",
                table: "PackingAllocations",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PackingAllocations_OutletId",
                table: "PackingAllocations",
                column: "OutletId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DishAllocations");

            migrationBuilder.DropTable(
                name: "PackingAllocations");
        }
    }
}
