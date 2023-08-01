using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_dishdetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_Dishes_ParentDishId",
                table: "Dishes");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_ParentDishId",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "ParentDishId",
                table: "Dishes");

            migrationBuilder.CreateTable(
                name: "DishDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ParentDishId = table.Column<int>(nullable: false),
                    DishId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishDetails_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishDetails_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishDetails_Dishes_ParentDishId",
                        column: x => x.ParentDishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishDetails_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DishDetails_CreatedBy",
                table: "DishDetails",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DishDetails_DishId",
                table: "DishDetails",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_DishDetails_ParentDishId",
                table: "DishDetails",
                column: "ParentDishId");

            migrationBuilder.CreateIndex(
                name: "IX_DishDetails_UpdatedBy",
                table: "DishDetails",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DishDetails");

            migrationBuilder.AddColumn<int>(
                name: "ParentDishId",
                table: "Dishes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_ParentDishId",
                table: "Dishes",
                column: "ParentDishId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_Dishes_ParentDishId",
                table: "Dishes",
                column: "ParentDishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
