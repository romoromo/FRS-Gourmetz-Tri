using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DeliveryBento : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int? DeliveryDetailId { get; set; }
        [ForeignKey("DeliveryDetailId")]
        public virtual DeliveryDetail DeliveryDetail { get; set; }

        public int? BentoAssetId { get; set; }
        [ForeignKey("BentoAssetId")]
        public virtual BentoAsset BentoAsset { get; set; }

        public int? DishId { get; set; }
        [ForeignKey("DishId")]
        public virtual Dish Dish { get; set; }

        public string UserData { get; set; }
    }
}
