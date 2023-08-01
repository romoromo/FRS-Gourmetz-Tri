using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class Addtokenpaymenttablesandrelated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TokenPaymentRequest",
                columns: table => new
                {
                    PaymentRequestId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    institutionId = table.Column<int>(nullable: false),
                    discountId = table.Column<long>(nullable: false),
                    priceId = table.Column<long>(nullable: false),
                    amount = table.Column<double>(nullable: false),
                    currencyCode = table.Column<string>(nullable: true),
                    mode = table.Column<string>(nullable: true),
                    subject = table.Column<string>(nullable: true),
                    notifyUrl = table.Column<string>(nullable: true),
                    returnUrl = table.Column<string>(nullable: true),
                    backUrl = table.Column<string>(nullable: true),
                    orderNo = table.Column<string>(nullable: true),
                    tokenOrderId = table.Column<long>(nullable: false),
                    sourceOfFunds = table.Column<string>(nullable: true),
                    transactionDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenPaymentRequest", x => x.PaymentRequestId);
                    table.ForeignKey(
                        name: "FK_TokenPaymentRequest_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenPaymentRequest_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TokenPaymentResponse",
                columns: table => new
                {
                    PaymentResponseId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    PaymentRequestId = table.Column<long>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    InstitutionId = table.Column<int>(nullable: false),
                    TokenOrderId = table.Column<int>(nullable: false),
                    IsSuccessful = table.Column<bool>(nullable: false),
                    IsCancelled = table.Column<bool>(nullable: false),
                    IsConfirmedPaid = table.Column<bool>(nullable: false),
                    StatusCode = table.Column<string>(nullable: true),
                    ResponseStatus = table.Column<string>(nullable: true),
                    Mode = table.Column<string>(nullable: true),
                    PaymentTransactionId = table.Column<string>(nullable: true),
                    OrderNo = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    Amount = table.Column<double>(nullable: false),
                    CurrencyCode = table.Column<string>(nullable: true),
                    PaymentUrl = table.Column<string>(nullable: true),
                    PaymentDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenPaymentResponse", x => x.PaymentResponseId);
                    table.ForeignKey(
                        name: "FK_TokenPaymentResponse_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenPaymentResponse_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TokenPaymentRequest_CreatedBy",
                table: "TokenPaymentRequest",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TokenPaymentRequest_UpdatedBy",
                table: "TokenPaymentRequest",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TokenPaymentResponse_CreatedBy",
                table: "TokenPaymentResponse",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TokenPaymentResponse_UpdatedBy",
                table: "TokenPaymentResponse",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TokenPaymentRequest");

            migrationBuilder.DropTable(
                name: "TokenPaymentResponse");
        }
    }
}
