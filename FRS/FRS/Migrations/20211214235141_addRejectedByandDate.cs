using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addRejectedByandDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RejectedBy",
                table: "SignagePublications",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedDate",
                table: "SignagePublications",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RejectedBy",
                table: "SignagePublicationHistorys",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedDate",
                table: "SignagePublicationHistorys",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SignagePublications_RejectedBy",
                table: "SignagePublications",
                column: "RejectedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SignagePublicationHistorys_RejectedBy",
                table: "SignagePublicationHistorys",
                column: "RejectedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_SignagePublicationHistorys_User_RejectedBy",
                table: "SignagePublicationHistorys",
                column: "RejectedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SignagePublications_User_RejectedBy",
                table: "SignagePublications",
                column: "RejectedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SignagePublicationHistorys_User_RejectedBy",
                table: "SignagePublicationHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_SignagePublications_User_RejectedBy",
                table: "SignagePublications");

            migrationBuilder.DropIndex(
                name: "IX_SignagePublications_RejectedBy",
                table: "SignagePublications");

            migrationBuilder.DropIndex(
                name: "IX_SignagePublicationHistorys_RejectedBy",
                table: "SignagePublicationHistorys");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                table: "SignagePublications");

            migrationBuilder.DropColumn(
                name: "RejectedDate",
                table: "SignagePublications");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                table: "SignagePublicationHistorys");

            migrationBuilder.DropColumn(
                name: "RejectedDate",
                table: "SignagePublicationHistorys");
        }
    }
}
