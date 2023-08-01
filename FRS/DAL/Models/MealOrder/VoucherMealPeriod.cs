using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class VoucherMealPeriod: AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Voucher")]
        public int VoucherId { get; set; }
        public virtual Voucher Voucher { get; set; }

        [ForeignKey("MealPeriod")]
        public int MealPeriodId { get; set; }
        public virtual MealPeriod MealPeriod { get; set; }
    }
}
