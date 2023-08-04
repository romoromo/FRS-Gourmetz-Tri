using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_studentgroup_cols : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "StudentGroups",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "StudentGroups",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "StudentGroups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "StudentGroups",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StudentGroupMealPlans",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    StudentGroupId = table.Column<int>(nullable: false),
                    MealSessionId = table.Column<int>(nullable: false),
                    DishId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentGroupMealPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentGroupMealPlans_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentGroupMealPlans_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentGroupMealPlans_MealSessions_MealSessionId",
                        column: x => x.MealSessionId,
                        principalTable: "MealSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentGroupMealPlans_StudentGroups_StudentGroupId",
                        column: x => x.StudentGroupId,
                        principalTable: "StudentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentGroupMealPlans_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupMealPlans_CreatedBy",
                table: "StudentGroupMealPlans",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupMealPlans_DishId",
                table: "StudentGroupMealPlans",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupMealPlans_MealSessionId",
                table: "StudentGroupMealPlans",
                column: "MealSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupMealPlans_StudentGroupId",
                table: "StudentGroupMealPlans",
                column: "StudentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupMealPlans_UpdatedBy",
                table: "StudentGroupMealPlans",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentGroupMealPlans");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "StudentGroups");
        }
    }
}
