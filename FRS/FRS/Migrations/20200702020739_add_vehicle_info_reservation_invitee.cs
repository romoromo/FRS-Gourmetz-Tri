using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class add_vehicle_info_reservation_invitee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CardType",
                table: "ReservationInvitees",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "ReservationInvitees",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IssueDate",
                table: "ReservationInvitees",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlateNumber",
                table: "ReservationInvitees",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardType",
                table: "ReservationInvitees");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "ReservationInvitees");

            migrationBuilder.DropColumn(
                name: "IssueDate",
                table: "ReservationInvitees");

            migrationBuilder.DropColumn(
                name: "PlateNumber",
                table: "ReservationInvitees");
        }
    }
}
