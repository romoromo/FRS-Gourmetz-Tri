using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class OutletMenuDish
    {
        [ForeignKey("MenuCycleSchedulePeriod")]
        public int MenuCycleSchedulePeriodId { get; set; }
        public virtual MenuCycleSchedulePeriod MenuCycleSchedulePeriod { get; set; }

        [ForeignKey("Menu")]
        public int MenuId { get; set; }
        public virtual Menu Menu { get; set; }

        [ForeignKey("Dish")]
        public int DishId { get; set; }
        public virtual Dish Dish { get; set; }

        [ForeignKey("MealType")]
        public int? MealTypeId { get; set; }
        public virtual MealType MealType { get; set; }

        [ForeignKey("Outlet")]
        public int? OutletId { get; set; }
        public virtual Outlet Outlet { get; set; }
    }
}
