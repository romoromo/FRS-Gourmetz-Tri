using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_dishrestrictions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BentoBoxTypeId",
                table: "Dishes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "Dishes",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Dishes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ParentDishId",
                table: "Dishes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DishRestriction",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DishId = table.Column<int>(nullable: false),
                    RestrictionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishRestriction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishRestriction_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishRestriction_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishRestriction_Restrictions_RestrictionId",
                        column: x => x.RestrictionId,
                        principalTable: "Restrictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishRestriction_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_BentoBoxTypeId",
                table: "Dishes",
                column: "BentoBoxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_FileId",
                table: "Dishes",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_ParentDishId",
                table: "Dishes",
                column: "ParentDishId");

            migrationBuilder.CreateIndex(
                name: "IX_DishRestriction_CreatedBy",
                table: "DishRestriction",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DishRestriction_DishId",
                table: "DishRestriction",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_DishRestriction_RestrictionId",
                table: "DishRestriction",
                column: "RestrictionId");

            migrationBuilder.CreateIndex(
                name: "IX_DishRestriction_UpdatedBy",
                table: "DishRestriction",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_BentoBoxTypes_BentoBoxTypeId",
                table: "Dishes",
                column: "BentoBoxTypeId",
                principalTable: "BentoBoxTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_Files_FileId",
                table: "Dishes",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_Dishes_ParentDishId",
                table: "Dishes",
                column: "ParentDishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_BentoBoxTypes_BentoBoxTypeId",
                table: "Dishes");

            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_Files_FileId",
                table: "Dishes");

            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_Dishes_ParentDishId",
                table: "Dishes");

            migrationBuilder.DropTable(
                name: "DishRestriction");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_BentoBoxTypeId",
                table: "Dishes");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_FileId",
                table: "Dishes");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_ParentDishId",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "BentoBoxTypeId",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "ParentDishId",
                table: "Dishes");
        }
    }
}
