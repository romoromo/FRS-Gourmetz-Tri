using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class Dish : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }
        public string Label { get; set; }
        public int DishTypeId { get; set; }
        public int? CuisineId { get; set; }

        public double RRPrice { get; set; }
        public double Cost { get; set; }
        public bool IsEnabled { get; set; }
        public int? FileId { get; set; }
        //public int? ParentDishId { get; set; }
        public int? BentoBoxTypeId { get; set; }
        public string ExtraNotes { get; set; }
        public float CookedWeight { get; set; }
        public float Protein { get; set; }
        public float Sugar { get; set; }
        public float TotalFat { get; set; }
        public float TotalCarb { get; set; }
        public float Calories { get; set; }
        //[ForeignKey("ParentDishId")]
        //public virtual Dish ParentDish { get; set; }

        public int? ProductionPictureId { get; set; }
        [ForeignKey("ProductionPictureId")]
        public virtual File ProductionPicture { get; set; }

        [ForeignKey("FileId")]
        public virtual File Icon { get; set; }

        [ForeignKey("DishTypeId")]
        public virtual DishType DishType { get; set; }

        [ForeignKey("CuisineId")]
        public virtual Cuisine Cuisine { get; set; }

        [ForeignKey("BentoBoxTypeId")]
        public virtual BentoBoxType BentoBoxType { get; set; }

        public string SapCode { get; set; }

        public int? StoreInfoId { get; set; }
        [ForeignKey("StoreInfoId")]
        public virtual StoreInfo StoreInfo { get; set; }

        public int? CatererId { get; set; }
        [ForeignKey("CatererId")]
        public virtual CatererInfo Caterer { get; set; }

        public virtual ICollection<DishDetail> SubDishes { get; set; }
        public virtual ICollection<DishPeriod> DishPeriods { get; set; }
        public virtual ICollection<DishRestriction> Restrictions { get; set; }
        public virtual ICollection<DishComponent> DishComponents { get; set; }
    }
}
