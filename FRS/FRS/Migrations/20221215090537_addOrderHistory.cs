using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addOrderHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TokensOrderHistoryId",
                table: "TokenOrdereds",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TokensOrderHistorys",
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
                    ProfileId = table.Column<int>(nullable: true),
                    DeliveryDate = table.Column<DateTime>(nullable: false),
                    PeriodId = table.Column<int>(nullable: true),
                    PaymentId = table.Column<int>(nullable: true),
                    TotalAmount = table.Column<float>(nullable: false),
                    PaymentTime = table.Column<DateTime>(nullable: false),
                    TotalPayment = table.Column<float>(nullable: false),
                    CancelRequestStatus = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    StudentGroupId = table.Column<int>(nullable: true),
                    MealSessionDetailId = table.Column<int>(nullable: true),
                    StoreId = table.Column<int>(nullable: true),
                    TokenOrderId = table.Column<int>(nullable: true),
                    Qty = table.Column<int>(nullable: true),
                    DishId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokensOrderHistorys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokensOrderHistorys_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokensOrderHistorys_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokensOrderHistorys_MealSessionDetails_MealSessionDetailId",
                        column: x => x.MealSessionDetailId,
                        principalTable: "MealSessionDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokensOrderHistorys_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokensOrderHistorys_MealPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "MealPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokensOrderHistorys_Students_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokensOrderHistorys_StoreInfos_StoreId",
                        column: x => x.StoreId,
                        principalTable: "StoreInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokensOrderHistorys_StudentGroups_StudentGroupId",
                        column: x => x.StudentGroupId,
                        principalTable: "StudentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokensOrderHistorys_TokenOrders_TokenOrderId",
                        column: x => x.TokenOrderId,
                        principalTable: "TokenOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokensOrderHistorys_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrdereds_TokensOrderHistoryId",
                table: "TokenOrdereds",
                column: "TokensOrderHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TokensOrderHistorys_CreatedBy",
                table: "TokensOrderHistorys",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TokensOrderHistorys_DishId",
                table: "TokensOrderHistorys",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_TokensOrderHistorys_MealSessionDetailId",
                table: "TokensOrderHistorys",
                column: "MealSessionDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_TokensOrderHistorys_PaymentId",
                table: "TokensOrderHistorys",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_TokensOrderHistorys_PeriodId",
                table: "TokensOrderHistorys",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_TokensOrderHistorys_ProfileId",
                table: "TokensOrderHistorys",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TokensOrderHistorys_StoreId",
                table: "TokensOrderHistorys",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_TokensOrderHistorys_StudentGroupId",
                table: "TokensOrderHistorys",
                column: "StudentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TokensOrderHistorys_TokenOrderId",
                table: "TokensOrderHistorys",
                column: "TokenOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TokensOrderHistorys_UpdatedBy",
                table: "TokensOrderHistorys",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_TokenOrdereds_TokensOrderHistorys_TokensOrderHistoryId",
                table: "TokenOrdereds",
                column: "TokensOrderHistoryId",
                principalTable: "TokensOrderHistorys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TokenOrdereds_TokensOrderHistorys_TokensOrderHistoryId",
                table: "TokenOrdereds");

            migrationBuilder.DropTable(
                name: "TokensOrderHistorys");

            migrationBuilder.DropIndex(
                name: "IX_TokenOrdereds_TokensOrderHistoryId",
                table: "TokenOrdereds");

            migrationBuilder.DropColumn(
                name: "TokensOrderHistoryId",
                table: "TokenOrdereds");
        }
    }
}
