using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class MealPlanOrder : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime TransactionTime { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? ProfileId { get; set; }

        [ForeignKey("ProfileId")]
        public virtual Student Student { get; set; }

        public decimal TotalAmount { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreInfo Store { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Status { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? StudentGroupId { get; set; }

        [ForeignKey("StudentGroupId")]
        public virtual StudentGroup StudentGroup { get; set; }

        public int? PaymentId { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; }

    }
}
