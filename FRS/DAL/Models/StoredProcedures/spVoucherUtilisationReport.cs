using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;

namespace DAL.Models.StoredProcedures
{
    [NotMapped]
    public class spVoucherUtilisationReport
    {
        [Key]
        public int Id { get; set; }
        public int Total { get; set; }
        public string VoucherName { get; set; }
        public string VoucherCode { get; set; }
        public float VoucherAmount { get; set; }
        public string DiscountType { get; set; }
        public DateTime ValidityStartDate { get; set; }
        public DateTime ValidityEndDate { get; set; }
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public DateTime UtilisedDate { get; set; }
        public string InvoiceNumber { get; set; }
        public float Discount { get; set; }
        public string VoucherStatus { get; set; }
    }
}
