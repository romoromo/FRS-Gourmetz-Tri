using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class add_token_order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TokenOrders",
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
                    SchoolId = table.Column<int>(nullable: false),
                    OrderProfile = table.Column<string>(nullable: true),
                    ProfileId = table.Column<int>(nullable: false),
                    DeliveryDate = table.Column<DateTime>(nullable: false),
                    PeriodId = table.Column<int>(nullable: false),
                    TotalAmount = table.Column<float>(nullable: false),
                    PaymentTime = table.Column<DateTime>(nullable: false),
                    TotalPayment = table.Column<float>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    StudentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokenOrders_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenOrders_MealPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "MealPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TokenOrders_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenOrders_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TokenOrdereds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    TokenId = table.Column<int>(nullable: false),
                    MealTypeId = table.Column<string>(nullable: true),
                    TokenDesc = table.Column<string>(nullable: true),
                    Qty = table.Column<int>(nullable: false),
                    OrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenOrdereds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokenOrdereds_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenOrdereds_TokenOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "TokenOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TokenOrdereds_MealTypes_TokenId",
                        column: x => x.TokenId,
                        principalTable: "MealTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TokenOrdereds_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrdereds_CreatedBy",
                table: "TokenOrdereds",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrdereds_OrderId",
                table: "TokenOrdereds",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrdereds_TokenId",
                table: "TokenOrdereds",
                column: "TokenId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrdereds_UpdatedBy",
                table: "TokenOrdereds",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrders_CreatedBy",
                table: "TokenOrders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrders_PeriodId",
                table: "TokenOrders",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrders_StudentId",
                table: "TokenOrders",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrders_UpdatedBy",
                table: "TokenOrders",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TokenOrdereds");

            migrationBuilder.DropTable(
                name: "TokenOrders");
        }
    }
}
