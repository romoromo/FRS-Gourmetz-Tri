using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class StoreInventory : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("StoreInfo")]
        public int StoreInfoId { get; set; }
        [ForeignKey("DeliveryOrder")]
        public int? DeliveryOrderID { get; set; }
        [ForeignKey("DeliveryOrderNew")]
        public int? DeliveryOrderNewID { get; set; }
        public string DeliveredBy { get; set; }
        public DateTime TimeReceived { get; set; }
        public string Remarks { get; set; }
        public virtual StoreInfo Store { get; set; }
        public virtual DeliveryOrder DeliveryOrder { get; set; }
        public virtual DeliveryOrderNew DeliveryOrderNew { get; set; }
        public virtual ICollection<StoreInventoryDetail> StoreInventoryDetails { get; set; }
    }
}
