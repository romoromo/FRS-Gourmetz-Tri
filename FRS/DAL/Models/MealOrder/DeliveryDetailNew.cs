using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DeliveryDetailNew : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int? DeliveryOrderId { get; set; }
        [ForeignKey("DeliveryOrderId")]
        public virtual DeliveryOrderNew DeliveryOrder { get; set; }

        public int? CartonAssetId { get; set; }
        [ForeignKey("CartonAssetId")]
        public virtual CartonAsset CartonAsset { get; set; }
        public string CartonAssetCode { get; set; }
        public string CartonType { get; set; }

        public int? TrackingStatusId { get; set; }
        [ForeignKey("TrackingStatusId")]
        public virtual TrackingStatus TrackingStatus { get; set; }

        //public int? DishId { get; set; }
        //[ForeignKey("DishId")]
        //public virtual Dish Dish { get; set; }

        //public int Qty { get; set; }

        public Boolean IsLoad { get; set; }
        public DateTime LoadingTime { get; set; }

        public virtual ICollection<DeliveryBentoNew> DeliveryBentos { get; set; }
    }
}
