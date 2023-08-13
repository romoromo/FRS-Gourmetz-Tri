using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addMealPlanOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "MealPlanOrders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    TransactionTime = table.Column<DateTime>(nullable: false),
                    ProfileId = table.Column<int>(nullable: true),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    StoreId = table.Column<int>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    StudentGroupId = table.Column<int>(nullable: true),
                    PaymentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlanOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealPlanOrders_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealPlanOrders_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealPlanOrders_Students_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealPlanOrders_StoreInfos_StoreId",
                        column: x => x.StoreId,
                        principalTable: "StoreInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealPlanOrders_StudentGroups_StudentGroupId",
                        column: x => x.StudentGroupId,
                        principalTable: "StudentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealPlanOrders_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanOrders_CreatedBy",
                table: "MealPlanOrders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanOrders_PaymentId",
                table: "MealPlanOrders",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanOrders_ProfileId",
                table: "MealPlanOrders",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanOrders_StoreId",
                table: "MealPlanOrders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanOrders_StudentGroupId",
                table: "MealPlanOrders",
                column: "StudentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanOrders_UpdatedBy",
                table: "MealPlanOrders",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealPlanOrders");

            
        }
    }
}
