using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class CartonAsset : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Code { get; set; }

        public int? InstitutionId { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        public int? CartonTypeId { get; set; }

        [ForeignKey("CartonTypeId")]
        public virtual CartonType CartonType { get; set; }

        public int? DishId { get; set; }

        [ForeignKey("DishId")]
        public virtual Dish Dish { get; set; }

        public int Qty { get; set; }

        public int? StoreInfoId { get; set; }

        [ForeignKey("StoreInfoId")]
        public virtual StoreInfo StoreInfo { get; set; }

        public int? ToStoreInfoId { get; set; }

        public virtual ICollection<BentoAsset> BentoAssets { get; set; }

        public virtual ICollection<CartonDisposableBox> DisposableBoxes { get; set; }

        public DateTime? Date { get; set; }

        public DateTime? TimeStamp { get; set; }
    }
}
