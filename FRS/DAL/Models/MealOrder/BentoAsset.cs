using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class BentoAsset : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Code { get; set; }

        public int? InstitutionId { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        public int? BentoBoxTypeId { get; set; }

        [ForeignKey("BentoBoxTypeId")]
        public virtual BentoBoxType BentoBoxType { get; set; }

        public int? CartonAssetId { get; set; }

        [ForeignKey("CartonAssetId")]
        public virtual CartonAsset CartonAsset { get; set; }

        public int? DishId { get; set; }

        [ForeignKey("DishId")]
        public virtual Dish Dish { get; set; }

        public int? StoreInfoId { get; set; }

        [ForeignKey("StoreInfoId")]
        public virtual StoreInfo StoreInfo { get; set; }

        public int? ToStoreInfoId { get; set; }

        public DateTime? Date { get; set; }

        public DateTime? TimeStamp { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public DateTime? LastPackingTime { get; set; }

        public DateTime? AssetRegistrationTime { get; set; }

        public DateTime? LastReturnTime { get; set; }

        public string Remarks { get; set; }

        public int? RouteId { get; set; }

        [ForeignKey("RouteId")]
        public virtual Route Route { get; set; }
    }
}
