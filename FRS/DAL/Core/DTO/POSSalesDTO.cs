using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DAL.Core.DTO
{
    public class POSResponse
    {
        public POSSalesResponse data { get; set; }
        public bool error { get; set; }
    }

    public class POSSalesResponse
    {
        public List<POSSalesDTO> sales { get; set; }
    }

    public class POSSalesDTO
    {
        public string id_gudang { get; set; }
        public string outlet_name { get; set; }
        public string no_invoice { get; set; }
        public string tgl_invoice { get; set; }
        public string id_jenis_harga { get; set; }
        public DateTime tgl_penjualan { get; set; }
        public string total_qty { get; set; }
        public string sub_total { get; set; }
        public string neto { get; set; }
        public string total_bayar { get; set; }
        public string jenis_bayar { get; set; }
        public string id_pos { get; set; }
        public string nama_customer { get; set; }
        public int student_id { get; set; }
        public string card_serial_number { get; set; }
        public int id_penjualan { get; set; }
        public virtual ICollection<SalesItem> sales_items { get; set; }
    }

    public class SalesItem
    {
        public string id_barang { get; set; }
        public int id_penjualan_detail { get; set; }
        public int id_penjualan { get; set; }
        public string harga_satuan { get; set; }
        public string qty { get; set; }
        public string harga_total { get; set; }
        public string diskon { get; set; }
        public string kode_barang { get; set; }
        public string nama_barang { get; set; }
        public string deskripsi { get; set; }
        public string delivery_date { get; set; }
    }



    public class InvoiceResponseDto
    {
        public InvoiceDataDto Data { get; set; }
        public bool Error { get; set; }
    }

    public class InvoiceDataDto
    {
        public POSSalesDTO Sales { get; set; }
        [JsonPropertyName("sales_items")]
        public List<SalesItem> SalesItems { get; set; }
    }
}
