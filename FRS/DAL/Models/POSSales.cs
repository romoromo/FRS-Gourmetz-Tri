using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class POSSales
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
        [Key]
        public int id_penjualan { get; set; }
        public virtual ICollection<SalesItem> sales_items { get; set; }

        public int PaymentID { get; set; }
        public DateTime SyncDate { get; set; }
    }

    public class SalesItem
    {
        public string id_barang { get; set; }
        [Key]
        public int id_penjualan_detail { get; set; }
        public int id_penjualan { get; set; }
        public string harga_satuan { get; set; }
        public string qty { get; set; }
        public string harga_total { get; set; }
        public string diskon { get; set; }
        public string kode_barang { get; set; }
        public string nama_barang { get; set; }
        public string deskripsi { get; set; }

        [ForeignKey("id_penjualan")]
        public virtual POSSales POSSales { get; set; }
    }
}
