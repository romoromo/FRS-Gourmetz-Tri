using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DeliveryBentoNew : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int? DeliveryDetailId { get; set; }
        [ForeignKey("DeliveryDetailId")]
        public virtual DeliveryDetailNew DeliveryDetail { get; set; }

        public int? BentoAssetId { get; set; }
        [ForeignKey("BentoAssetId")]
        public virtual BentoAsset BentoAsset { get; set; }
        public string BentoAssetCode { get; set; }
        public string BentoType { get; set; }

        public int? DishId { get; set; }
        [ForeignKey("DishId")]
        public virtual Dish Dish { get; set; }
        public string DishCode { get; set; }
        public string DishLabel { get; set; }
        public string DishType { get; set; }

        public int Qty { get; set; }
        public int ReceivedQty { get; set; }

        public string UserData { get; set; }
    }
}
