using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class Voucher : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? InstitutionId { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int VoucherTypeId { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime StartDateTime { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime EndDateTime { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string DiscountType { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public double DiscountAmount { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public double MinimumBasketPrice { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int UsageQuantity { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int UsageQuantityUsed { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int MaxDistribution { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public bool IsDisplayAllPages { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Code { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? OutletProfileId { get; set; }

        [ForeignKey("VoucherTypeId")]
        public virtual VoucherType VoucherType { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        public virtual ICollection<VoucherMealPeriod> VoucherMealPeriods { get; set; }
        public virtual ICollection<VoucherDish> VoucherDishes { get; set; }
    }
}
