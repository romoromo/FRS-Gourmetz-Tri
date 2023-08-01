using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class storeinventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoreInventories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    StoreInfoId = table.Column<int>(nullable: false),
                    DeliveryOrderID = table.Column<int>(nullable: false),
                    DeliveredBy = table.Column<string>(nullable: true),
                    TimeReceived = table.Column<DateTime>(nullable: false),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreInventories_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreInventories_DeliveryOrders_DeliveryOrderID",
                        column: x => x.DeliveryOrderID,
                        principalTable: "DeliveryOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreInventories_StoreInfos_StoreInfoId",
                        column: x => x.StoreInfoId,
                        principalTable: "StoreInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreInventories_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StoreInventoryDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    StoreInventoryId = table.Column<int>(nullable: true),
                    DishId = table.Column<int>(nullable: true),
                    QtyExpected = table.Column<int>(nullable: false),
                    QtyReceived = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreInventoryDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreInventoryDetails_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreInventoryDetails_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreInventoryDetails_StoreInventories_StoreInventoryId",
                        column: x => x.StoreInventoryId,
                        principalTable: "StoreInventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreInventoryDetails_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreInventories_CreatedBy",
                table: "StoreInventories",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StoreInventories_DeliveryOrderID",
                table: "StoreInventories",
                column: "DeliveryOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_StoreInventories_StoreInfoId",
                table: "StoreInventories",
                column: "StoreInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreInventories_UpdatedBy",
                table: "StoreInventories",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StoreInventoryDetails_CreatedBy",
                table: "StoreInventoryDetails",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StoreInventoryDetails_DishId",
                table: "StoreInventoryDetails",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreInventoryDetails_StoreInventoryId",
                table: "StoreInventoryDetails",
                column: "StoreInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreInventoryDetails_UpdatedBy",
                table: "StoreInventoryDetails",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreInventoryDetails");

            migrationBuilder.DropTable(
                name: "StoreInventories");
        }
    }
}
