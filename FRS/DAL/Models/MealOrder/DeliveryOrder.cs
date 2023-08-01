using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DeliveryOrder : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string DONumber { get; set; }

        public int? InstitutionId { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string DeliveryAddress { get; set; }

        public int? CatererInfoId { get; set; }

        [ForeignKey("CatererInfoId")]
        public virtual CatererInfo CatererInfo { get; set; }

        public int? FromStoreId { get; set; }

        [ForeignKey("FromStoreId")]
        public virtual StoreInfo FromStore { get; set; }

        public int? ToStoreId { get; set; }

        [ForeignKey("ToStoreId")]
        public virtual StoreInfo ToStore { get; set; }

        public bool Completed { get; set; }

        public virtual ICollection<DeliveryDetail> DeliveryDetails { get; set; }

        [ForeignKey("ClosedBy")]
        public virtual ApplicationUser ClosedByUser { get; set; }

        [Sieve(CanFilter = true)]
        public int? ClosedBy { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime ClosedDate { get; set; }

        public DateTime FoodExpiryDate { get; set; }
    }
}
