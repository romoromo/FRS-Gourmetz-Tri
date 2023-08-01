using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_menugroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OutletProfileId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuGroups_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuGroups_OutletProfiles_OutletProfileId",
                        column: x => x.OutletProfileId,
                        principalTable: "OutletProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuGroups_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MenuGroupClasses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ClassId = table.Column<int>(nullable: false),
                    MenuGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuGroupClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuGroupClasses_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuGroupClasses_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuGroupClasses_MenuGroups_MenuGroupId",
                        column: x => x.MenuGroupId,
                        principalTable: "MenuGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuGroupClasses_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MenuGroupDishCycles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DishCycleId = table.Column<int>(nullable: false),
                    MenuGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuGroupDishCycles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuGroupDishCycles_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuGroupDishCycles_DishCycles_DishCycleId",
                        column: x => x.DishCycleId,
                        principalTable: "DishCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuGroupDishCycles_MenuGroups_MenuGroupId",
                        column: x => x.MenuGroupId,
                        principalTable: "MenuGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuGroupDishCycles_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuGroupClasses_ClassId",
                table: "MenuGroupClasses",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuGroupClasses_CreatedBy",
                table: "MenuGroupClasses",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MenuGroupClasses_MenuGroupId",
                table: "MenuGroupClasses",
                column: "MenuGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuGroupClasses_UpdatedBy",
                table: "MenuGroupClasses",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MenuGroupDishCycles_CreatedBy",
                table: "MenuGroupDishCycles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MenuGroupDishCycles_DishCycleId",
                table: "MenuGroupDishCycles",
                column: "DishCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuGroupDishCycles_MenuGroupId",
                table: "MenuGroupDishCycles",
                column: "MenuGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuGroupDishCycles_UpdatedBy",
                table: "MenuGroupDishCycles",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MenuGroups_CreatedBy",
                table: "MenuGroups",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MenuGroups_OutletProfileId",
                table: "MenuGroups",
                column: "OutletProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuGroups_UpdatedBy",
                table: "MenuGroups",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuGroupClasses");

            migrationBuilder.DropTable(
                name: "MenuGroupDishCycles");

            migrationBuilder.DropTable(
                name: "MenuGroups");
        }
    }
}
