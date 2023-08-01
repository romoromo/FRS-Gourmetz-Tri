using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addQueueLoh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QueueLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Queueid = table.Column<string>(nullable: true),
                    CallAction = table.Column<string>(nullable: true),
                    QueueNo = table.Column<string>(nullable: true),
                    StationId = table.Column<string>(nullable: true),
                    MessageId = table.Column<string>(nullable: true),
                    InstitutionId = table.Column<string>(nullable: true),
                    ClinicId = table.Column<string>(nullable: true),
                    TerminalId = table.Column<string>(nullable: true),
                    TerminalName = table.Column<string>(nullable: true),
                    QueueNumber = table.Column<string>(nullable: true),
                    Timestamp = table.Column<string>(nullable: true),
                    isSuccess = table.Column<bool>(nullable: false),
                    resultMessage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueueLog_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QueueLog_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QueueLog_CreatedBy",
                table: "QueueLog",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QueueLog_UpdatedBy",
                table: "QueueLog",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QueueLog");
        }
    }
}
