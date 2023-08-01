using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DishCyclePeriod : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("DishCycle")]
        public int DishCycleId { get; set; }
        public virtual DishCycle DishCycle { get; set; }

        [ForeignKey("MealPeriod")]
        public int MealPeriodId { get; set; }
        public virtual MealPeriod MealPeriod { get; set; }

        public virtual ICollection<OutletDishCyclePeriodMenu> ExcludedMenus { get; set; }
    }
}
