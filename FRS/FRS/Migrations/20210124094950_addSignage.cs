using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addSignage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SignageCompilations",
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
                    Description = table.Column<string>(nullable: true),
                    width = table.Column<int>(nullable: false),
                    height = table.Column<int>(nullable: false),
                    BackgroundColor = table.Column<string>(nullable: true),
                    BackgroundImage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignageCompilations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignageCompilations_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignageCompilations_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SignageComponents",
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
                    Description = table.Column<string>(nullable: true),
                    FixWidth = table.Column<int>(nullable: true),
                    FixHeight = table.Column<int>(nullable: true),
                    IsRatio = table.Column<bool>(nullable: false),
                    BackgroundColor = table.Column<string>(nullable: true),
                    BackgroundImage = table.Column<string>(nullable: true),
                    ComponentType = table.Column<string>(nullable: true),
                    Configurations = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignageComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignageComponents_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignageComponents_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SignagePublications",
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
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignagePublications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignagePublications_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignagePublications_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SignageCompilationComponents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ComponentId = table.Column<int>(nullable: true),
                    CompilationId = table.Column<int>(nullable: true),
                    x = table.Column<int>(nullable: false),
                    y = table.Column<int>(nullable: false),
                    width = table.Column<int>(nullable: false),
                    height = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignageCompilationComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignageCompilationComponents_SignageCompilations_CompilationId",
                        column: x => x.CompilationId,
                        principalTable: "SignageCompilations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignageCompilationComponents_SignageComponents_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "SignageComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignageCompilationComponents_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignageCompilationComponents_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SignageSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    PublicationId = table.Column<int>(nullable: true),
                    EffectiveDate = table.Column<DateTime>(nullable: true),
                    IneffectiveDate = table.Column<DateTime>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: true),
                    Monday = table.Column<bool>(nullable: false),
                    Tuesday = table.Column<bool>(nullable: false),
                    Wednesday = table.Column<bool>(nullable: false),
                    Thursday = table.Column<bool>(nullable: false),
                    Friday = table.Column<bool>(nullable: false),
                    Saturday = table.Column<bool>(nullable: false),
                    Sunday = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignageSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignageSchedules_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignageSchedules_SignagePublications_PublicationId",
                        column: x => x.PublicationId,
                        principalTable: "SignagePublications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignageSchedules_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SignageScheduleCompilations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ScheduleId = table.Column<int>(nullable: true),
                    CompilationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignageScheduleCompilations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignageScheduleCompilations_SignageCompilations_CompilationId",
                        column: x => x.CompilationId,
                        principalTable: "SignageCompilations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignageScheduleCompilations_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignageScheduleCompilations_SignageSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "SignageSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignageScheduleCompilations_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SignageCompilationComponents_CompilationId",
                table: "SignageCompilationComponents",
                column: "CompilationId");

            migrationBuilder.CreateIndex(
                name: "IX_SignageCompilationComponents_ComponentId",
                table: "SignageCompilationComponents",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_SignageCompilationComponents_CreatedBy",
                table: "SignageCompilationComponents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SignageCompilationComponents_UpdatedBy",
                table: "SignageCompilationComponents",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SignageCompilations_CreatedBy",
                table: "SignageCompilations",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SignageCompilations_UpdatedBy",
                table: "SignageCompilations",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SignageComponents_CreatedBy",
                table: "SignageComponents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SignageComponents_UpdatedBy",
                table: "SignageComponents",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SignagePublications_CreatedBy",
                table: "SignagePublications",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SignagePublications_UpdatedBy",
                table: "SignagePublications",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SignageScheduleCompilations_CompilationId",
                table: "SignageScheduleCompilations",
                column: "CompilationId");

            migrationBuilder.CreateIndex(
                name: "IX_SignageScheduleCompilations_CreatedBy",
                table: "SignageScheduleCompilations",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SignageScheduleCompilations_ScheduleId",
                table: "SignageScheduleCompilations",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_SignageScheduleCompilations_UpdatedBy",
                table: "SignageScheduleCompilations",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SignageSchedules_CreatedBy",
                table: "SignageSchedules",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SignageSchedules_PublicationId",
                table: "SignageSchedules",
                column: "PublicationId");

            migrationBuilder.CreateIndex(
                name: "IX_SignageSchedules_UpdatedBy",
                table: "SignageSchedules",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SignageCompilationComponents");

            migrationBuilder.DropTable(
                name: "SignageScheduleCompilations");

            migrationBuilder.DropTable(
                name: "SignageComponents");

            migrationBuilder.DropTable(
                name: "SignageCompilations");

            migrationBuilder.DropTable(
                name: "SignageSchedules");

            migrationBuilder.DropTable(
                name: "SignagePublications");
        }
    }
}
