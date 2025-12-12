using DAL.Core.Audit.Attributes;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using DAL.Models.MealOrder;

namespace DAL.Models
{
    public class StudentWalletTransaction : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int StudentId { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public double Amount { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string TransactionType { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Description { get; set; }

        public string Remarks { get; set; }


        [ForeignKey("StudentId")]
        public virtual Student student { get; set; }

        public int? PaymentId { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; }


        public int? TokenOrderId { get; set; }
        [ForeignKey("TokenOrderId")]
        public virtual TokenOrder TokenOrder { get; set; }
    }
}
