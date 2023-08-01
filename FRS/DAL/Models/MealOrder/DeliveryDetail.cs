using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DeliveryDetail : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int? DeliveryOrderId { get; set; }
        [ForeignKey("DeliveryOrderId")]
        public virtual DeliveryOrder DeliveryOrder { get; set; }

        public int? CartonAssetId { get; set; }
        [ForeignKey("CartonAssetId")]
        public virtual CartonAsset CartonAsset { get; set; }

        public int? TrackingStatusId { get; set; }
        [ForeignKey("TrackingStatusId")]
        public virtual TrackingStatus TrackingStatus { get; set; }

        public virtual ICollection<DeliveryBento> DeliveryBentos { get; set; }
    }
}
