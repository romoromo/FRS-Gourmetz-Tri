using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class mealallocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MealAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    deliveryDate = table.Column<DateTime>(nullable: true),
                    periodId = table.Column<int>(nullable: true),
                    mealSessionId = table.Column<int>(nullable: true),
                    outletId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealAllocations_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealAllocations_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealAllocations_MealSessionDetails_mealSessionId",
                        column: x => x.mealSessionId,
                        principalTable: "MealSessionDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealAllocations_Outlets_outletId",
                        column: x => x.outletId,
                        principalTable: "Outlets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealAllocations_MealPeriods_periodId",
                        column: x => x.periodId,
                        principalTable: "MealPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TokenLabels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    meal_allocation_id = table.Column<int>(nullable: true),
                    token_id = table.Column<int>(nullable: true),
                    token_name = table.Column<string>(nullable: true),
                    qty = table.Column<int>(nullable: true),
                    qty_dishes = table.Column<int>(nullable: true),
                    qty_pdishes = table.Column<int>(nullable: true),
                    qty_tdishes = table.Column<int>(nullable: true),
                    deliveryDate = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenLabels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokenLabels_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenLabels_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenLabels_MealAllocations_meal_allocation_id",
                        column: x => x.meal_allocation_id,
                        principalTable: "MealAllocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenLabels_MealTypes_token_id",
                        column: x => x.token_id,
                        principalTable: "MealTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TokenDishLabels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    token_label_id = table.Column<int>(nullable: true),
                    token_id = table.Column<int>(nullable: true),
                    token_name = table.Column<string>(nullable: true),
                    dish_id = table.Column<int>(nullable: true),
                    dish_name = table.Column<string>(nullable: true),
                    dish_code = table.Column<string>(nullable: true),
                    o_qty = table.Column<int>(nullable: true),
                    p_qty = table.Column<int>(nullable: true),
                    a_qty = table.Column<int>(nullable: true),
                    t_qty = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenDishLabels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokenDishLabels_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenDishLabels_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenDishLabels_MealTypes_token_id",
                        column: x => x.token_id,
                        principalTable: "MealTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenDishLabels_TokenLabels_token_label_id",
                        column: x => x.token_label_id,
                        principalTable: "TokenLabels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealAllocations_CreatedBy",
                table: "MealAllocations",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MealAllocations_UpdatedBy",
                table: "MealAllocations",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MealAllocations_mealSessionId",
                table: "MealAllocations",
                column: "mealSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_MealAllocations_outletId",
                table: "MealAllocations",
                column: "outletId");

            migrationBuilder.CreateIndex(
                name: "IX_MealAllocations_periodId",
                table: "MealAllocations",
                column: "periodId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenDishLabels_CreatedBy",
                table: "TokenDishLabels",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TokenDishLabels_UpdatedBy",
                table: "TokenDishLabels",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TokenDishLabels_token_id",
                table: "TokenDishLabels",
                column: "token_id");

            migrationBuilder.CreateIndex(
                name: "IX_TokenDishLabels_token_label_id",
                table: "TokenDishLabels",
                column: "token_label_id");

            migrationBuilder.CreateIndex(
                name: "IX_TokenLabels_CreatedBy",
                table: "TokenLabels",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TokenLabels_UpdatedBy",
                table: "TokenLabels",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TokenLabels_meal_allocation_id",
                table: "TokenLabels",
                column: "meal_allocation_id");

            migrationBuilder.CreateIndex(
                name: "IX_TokenLabels_token_id",
                table: "TokenLabels",
                column: "token_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TokenDishLabels");

            migrationBuilder.DropTable(
                name: "TokenLabels");

            migrationBuilder.DropTable(
                name: "MealAllocations");
        }
    }
}
