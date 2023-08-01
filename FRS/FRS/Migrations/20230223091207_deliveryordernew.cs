using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class deliveryordernew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliveryOrderNews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DONumber = table.Column<string>(nullable: true),
                    InstitutionId = table.Column<int>(nullable: true),
                    DeliveryAddress = table.Column<string>(nullable: true),
                    CatererInfoId = table.Column<int>(nullable: true),
                    FromStoreId = table.Column<int>(nullable: true),
                    ToStoreId = table.Column<int>(nullable: true),
                    Completed = table.Column<bool>(nullable: false),
                    ClosedBy = table.Column<int>(nullable: true),
                    ClosedDate = table.Column<DateTime>(nullable: false),
                    FoodExpiryDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryOrderNews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryOrderNews_CatererInfos_CatererInfoId",
                        column: x => x.CatererInfoId,
                        principalTable: "CatererInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryOrderNews_User_ClosedBy",
                        column: x => x.ClosedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryOrderNews_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryOrderNews_StoreInfos_FromStoreId",
                        column: x => x.FromStoreId,
                        principalTable: "StoreInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryOrderNews_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryOrderNews_StoreInfos_ToStoreId",
                        column: x => x.ToStoreId,
                        principalTable: "StoreInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryOrderNews_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryDetailNews",
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
                    TrackingStatusId = table.Column<int>(nullable: true),
                    DishId = table.Column<int>(nullable: true),
                    Qty = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryDetailNews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryDetailNews_CartonAssets_CartonAssetId",
                        column: x => x.CartonAssetId,
                        principalTable: "CartonAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryDetailNews_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryDetailNews_DeliveryOrderNews_DeliveryOrderId",
                        column: x => x.DeliveryOrderId,
                        principalTable: "DeliveryOrderNews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryDetailNews_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryDetailNews_TrackingStatuss_TrackingStatusId",
                        column: x => x.TrackingStatusId,
                        principalTable: "TrackingStatuss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryDetailNews_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryBentoNews",
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
                    UserData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryBentoNews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryBentoNews_BentoAssets_BentoAssetId",
                        column: x => x.BentoAssetId,
                        principalTable: "BentoAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryBentoNews_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryBentoNews_DeliveryDetailNews_DeliveryDetailId",
                        column: x => x.DeliveryDetailId,
                        principalTable: "DeliveryDetailNews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryBentoNews_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryBentoNews_BentoAssetId",
                table: "DeliveryBentoNews",
                column: "BentoAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryBentoNews_CreatedBy",
                table: "DeliveryBentoNews",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryBentoNews_DeliveryDetailId",
                table: "DeliveryBentoNews",
                column: "DeliveryDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryBentoNews_UpdatedBy",
                table: "DeliveryBentoNews",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetailNews_CartonAssetId",
                table: "DeliveryDetailNews",
                column: "CartonAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetailNews_CreatedBy",
                table: "DeliveryDetailNews",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetailNews_DeliveryOrderId",
                table: "DeliveryDetailNews",
                column: "DeliveryOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetailNews_DishId",
                table: "DeliveryDetailNews",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetailNews_TrackingStatusId",
                table: "DeliveryDetailNews",
                column: "TrackingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetailNews_UpdatedBy",
                table: "DeliveryDetailNews",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrderNews_CatererInfoId",
                table: "DeliveryOrderNews",
                column: "CatererInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrderNews_ClosedBy",
                table: "DeliveryOrderNews",
                column: "ClosedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrderNews_CreatedBy",
                table: "DeliveryOrderNews",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrderNews_FromStoreId",
                table: "DeliveryOrderNews",
                column: "FromStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrderNews_InstitutionId",
                table: "DeliveryOrderNews",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrderNews_ToStoreId",
                table: "DeliveryOrderNews",
                column: "ToStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrderNews_UpdatedBy",
                table: "DeliveryOrderNews",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryBentoNews");

            migrationBuilder.DropTable(
                name: "DeliveryDetailNews");

            migrationBuilder.DropTable(
                name: "DeliveryOrderNews");
        }
    }
}
