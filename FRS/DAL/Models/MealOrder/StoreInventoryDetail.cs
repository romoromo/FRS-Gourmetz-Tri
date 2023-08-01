using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class StoreInventoryDetail : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int? StoreInventoryId { get; set; }
        [ForeignKey("StoreInventoryId")]
        public virtual StoreInventory StoreInventory { get; set; }

        public int? DishId { get; set; }
        [ForeignKey("DishId")]
        public virtual Dish Dish { get; set; }

        public int? CartonId { get; set; }
        [ForeignKey("CartonId")]
        public virtual CartonAsset Carton { get; set; }

        [ForeignKey("StoreInfo")]
        public int? StoreInfoId { get; set; }
        public virtual StoreInfo Store { get; set; }

        public string Remarks { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime TimeReceived { get; set; }

        public int QtyExpected { get; set; }

        public int QtyReceived { get; set; }
    }
}
