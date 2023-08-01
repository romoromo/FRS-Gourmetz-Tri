using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addDeliveryDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliveryDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DeliveryOrderId = table.Column<int>(nullable: true),
                    CartonAssetId = table.Column<int>(nullable: true),
                    TrackingStatusId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryDetails_CartonAssets_CartonAssetId",
                        column: x => x.CartonAssetId,
                        principalTable: "CartonAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryDetails_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryDetails_DeliveryOrders_DeliveryOrderId",
                        column: x => x.DeliveryOrderId,
                        principalTable: "DeliveryOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryDetails_TrackingStatuss_TrackingStatusId",
                        column: x => x.TrackingStatusId,
                        principalTable: "TrackingStatuss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryDetails_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryBentos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DeliveryDetailId = table.Column<int>(nullable: true),
                    BentoAssetId = table.Column<int>(nullable: true),
                    DishId = table.Column<int>(nullable: true),
                    UserData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryBentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryBentos_BentoAssets_BentoAssetId",
                        column: x => x.BentoAssetId,
                        principalTable: "BentoAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryBentos_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryBentos_DeliveryDetails_DeliveryDetailId",
                        column: x => x.DeliveryDetailId,
                        principalTable: "DeliveryDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryBentos_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryBentos_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryBentos_BentoAssetId",
                table: "DeliveryBentos",
                column: "BentoAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryBentos_CreatedBy",
                table: "DeliveryBentos",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryBentos_DeliveryDetailId",
                table: "DeliveryBentos",
                column: "DeliveryDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryBentos_DishId",
                table: "DeliveryBentos",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryBentos_UpdatedBy",
                table: "DeliveryBentos",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetails_CartonAssetId",
                table: "DeliveryDetails",
                column: "CartonAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetails_CreatedBy",
                table: "DeliveryDetails",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetails_DeliveryOrderId",
                table: "DeliveryDetails",
                column: "DeliveryOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetails_TrackingStatusId",
                table: "DeliveryDetails",
                column: "TrackingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetails_UpdatedBy",
                table: "DeliveryDetails",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryBentos");

            migrationBuilder.DropTable(
                name: "DeliveryDetails");
        }
    }
}
