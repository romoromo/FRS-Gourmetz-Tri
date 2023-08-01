using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_dishcomponent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Calories",
                table: "Dishes",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateTable(
                name: "DishComponents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Category = table.Column<string>(nullable: true),
                    Component = table.Column<string>(nullable: true),
                    Weight = table.Column<float>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    SapProductCode = table.Column<string>(nullable: true),
                    DishId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishComponents_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishComponents_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishComponents_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DishComponents_CreatedBy",
                table: "DishComponents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DishComponents_DishId",
                table: "DishComponents",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_DishComponents_UpdatedBy",
                table: "DishComponents",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DishComponents");

            migrationBuilder.DropColumn(
                name: "Calories",
                table: "Dishes");
        }
    }
}
