using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class OutletDishCyclePeriodMenu : AuditableEntity
    {
        [ForeignKey("DishCyclePeriod")]
        public int DishCyclePeriodId { get; set; }
        public virtual DishCyclePeriod DishCyclePeriod { get; set; }

        [ForeignKey("Dish")]
        public int DishId { get; set; }
        public virtual Dish Dish { get; set; }

        [ForeignKey("DishCycleScheduleSet")]
        public int DishCycleScheduleSetId { get; set; }
        public virtual DishCycleScheduleSet DishCycleScheduleSet { get; set; }

        [ForeignKey("Outlet")]
        public int OutletId { get; set; }
        public virtual Outlet Outlet { get; set; }
    }
}
