using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class add_order_portalcontent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderPortalContents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Announcement = table.Column<string>(nullable: true),
                    EffectiveDate = table.Column<DateTime>(nullable: false),
                    OutletId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPortalContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPortalContents_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderPortalContents_Outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderPortalContents_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderPortalBanners",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Subtitle = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    ImageId = table.Column<int>(nullable: true),
                    OrderPortalContentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPortalBanners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPortalBanners_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderPortalBanners_Files_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderPortalBanners_OrderPortalContents_OrderPortalContentId",
                        column: x => x.OrderPortalContentId,
                        principalTable: "OrderPortalContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderPortalBanners_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderPortalBanners_CreatedBy",
                table: "OrderPortalBanners",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPortalBanners_ImageId",
                table: "OrderPortalBanners",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPortalBanners_OrderPortalContentId",
                table: "OrderPortalBanners",
                column: "OrderPortalContentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPortalBanners_UpdatedBy",
                table: "OrderPortalBanners",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPortalContents_CreatedBy",
                table: "OrderPortalContents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPortalContents_OutletId",
                table: "OrderPortalContents",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPortalContents_UpdatedBy",
                table: "OrderPortalContents",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderPortalBanners");

            migrationBuilder.DropTable(
                name: "OrderPortalContents");
        }
    }
}
