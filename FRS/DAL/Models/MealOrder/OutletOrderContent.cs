using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class OrderPortalContent : AuditableEntity
    {
        [Key]
        [Sieve(CanFilter = true, CanSort = true)]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Announcement { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Description { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime EffectiveStartDate { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime EffectiveEndDate { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? OutletId { get; set; }

        [ForeignKey("OutletId")]
        public virtual Outlet Outlet { get; set; }

        public virtual ICollection<OrderPortalBanner> Banners { get; set; }
    }
}
