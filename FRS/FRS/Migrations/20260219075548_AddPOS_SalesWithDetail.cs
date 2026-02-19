using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddPOS_SalesWithDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "POSSales",
                columns: table => new
                {
                    id_penjualan = table.Column<int>(type: "int", nullable: false),
                    id_gudang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    outlet_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    no_invoice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tgl_invoice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_jenis_harga = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tgl_penjualan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    total_qty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sub_total = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    neto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    total_bayar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    jenis_bayar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_pos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nama_customer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    card_serial_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentID = table.Column<int>(type: "int", nullable: false),
                    SyncDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POSSales", x => x.id_penjualan);
                });

            migrationBuilder.CreateTable(
                name: "POSSalesItem",
                columns: table => new
                {
                    id_penjualan_detail = table.Column<int>(type: "int", nullable: false),
                    id_barang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_penjualan = table.Column<int>(type: "int", nullable: false),
                    harga_satuan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    qty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    harga_total = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    diskon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    kode_barang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nama_barang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    deskripsi = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POSSalesItem", x => x.id_penjualan_detail);
                    table.ForeignKey(
                        name: "FK_POSSalesItem_POSSales_id_penjualan",
                        column: x => x.id_penjualan,
                        principalTable: "POSSales",
                        principalColumn: "id_penjualan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_POSSalesItem_id_penjualan",
                table: "POSSalesItem",
                column: "id_penjualan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "POSSalesItem");

            migrationBuilder.DropTable(
                name: "POSSales");
        }
    }
}
