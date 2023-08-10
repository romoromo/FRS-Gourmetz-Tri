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
        public int Id { get; set; }

        public string Announcement { get; set; }

        public DateTime EffectiveDate { get; set; }

        public int? OutletId { get; set; }

        [ForeignKey("OutletId")]
        public virtual Outlet Outlet { get; set; }

        public virtual ICollection<OrderPortalBanner> Banners { get; set; }
    }
}
