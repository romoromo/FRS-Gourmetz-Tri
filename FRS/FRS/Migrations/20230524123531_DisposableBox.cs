using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class DisposableBox : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartonDisposableBoxes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    InstitutionId = table.Column<int>(nullable: true),
                    CartonAssetId = table.Column<int>(nullable: true),
                    DishId = table.Column<int>(nullable: true),
                    Qty = table.Column<int>(nullable: false),
                    StoreInfoId = table.Column<int>(nullable: true),
                    ToStoreInfoId = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartonDisposableBoxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartonDisposableBoxes_CartonAssets_CartonAssetId",
                        column: x => x.CartonAssetId,
                        principalTable: "CartonAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartonDisposableBoxes_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartonDisposableBoxes_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartonDisposableBoxes_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartonDisposableBoxes_StoreInfos_StoreInfoId",
                        column: x => x.StoreInfoId,
                        principalTable: "StoreInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartonDisposableBoxes_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartonDisposableBoxes_CartonAssetId",
                table: "CartonDisposableBoxes",
                column: "CartonAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_CartonDisposableBoxes_CreatedBy",
                table: "CartonDisposableBoxes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CartonDisposableBoxes_DishId",
                table: "CartonDisposableBoxes",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_CartonDisposableBoxes_InstitutionId",
                table: "CartonDisposableBoxes",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_CartonDisposableBoxes_StoreInfoId",
                table: "CartonDisposableBoxes",
                column: "StoreInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_CartonDisposableBoxes_UpdatedBy",
                table: "CartonDisposableBoxes",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartonDisposableBoxes");
        }
    }
}
