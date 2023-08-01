using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_smartroom_tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SmartRoomResources",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlternateID = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    FloorID = table.Column<int>(nullable: true),
                    Floor = table.Column<string>(nullable: true),
                    ExchangeID = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    IP = table.Column<string>(nullable: true),
                    DeviceName = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    FloorLocationX = table.Column<int>(nullable: true),
                    FloorLocationY = table.Column<int>(nullable: true),
                    SeatingCapacity = table.Column<int>(nullable: false),
                    Access = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartRoomResources", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SmartRoomSchedules",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlternateID = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true),
                    RoomID = table.Column<int>(nullable: true),
                    AlternateRoomID = table.Column<string>(nullable: true),
                    UserID = table.Column<int>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    MeetingContactNo = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: true),
                    ReserveSource = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    StatusRemarks = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    RequestorID = table.Column<string>(nullable: true),
                    Requestor = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    StartedBy = table.Column<int>(nullable: true),
                    StartedAt = table.Column<DateTime>(nullable: true),
                    EndedBy = table.Column<int>(nullable: true),
                    EndedAt = table.Column<DateTime>(nullable: true),
                    LastUpdatedDateTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartRoomSchedules", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SmartRoomResources");

            migrationBuilder.DropTable(
                name: "SmartRoomSchedules");
        }
    }
}
