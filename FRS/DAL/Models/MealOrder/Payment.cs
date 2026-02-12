using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class Payment : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public string InvoiceNumber { get; set; }

        public virtual ICollection<TokenOrder> TokenOrders { get; set; }

        public virtual ICollection<MealPlanOrder> MealPlanOrders { get; set; }

        public decimal subtotal { get; set; }
        public decimal discount { get; set; }
        public decimal gst { get; set; }
        public decimal subdisctotal { get; set; }
        public decimal transactionFee { get; set; }
        public decimal fixedTransactionFee { get; set; }
        public decimal total { get; set; }

        public int? VoucherId { get; set; }

        [ForeignKey("VoucherId")]
        public virtual Voucher Voucher { get; set; }

        public int? PaymentTypeId { get; set; }

        [ForeignKey("PaymentTypeId")]
        public virtual PaymentType PaymentType { get; set; }


        public int? InstitutionId { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        public string email { get; set; }

        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public int? StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        public string Status { get; set; }


        public string PaymentNumber { get; set; }

        public string fomoid { get; set; }
        public bool invoiceSent { get; set; }

        public string version { get; set; }

        public string PosInvoiceId { get; set; }

    }
}
