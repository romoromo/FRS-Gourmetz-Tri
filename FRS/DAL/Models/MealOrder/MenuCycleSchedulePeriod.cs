using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class MenuCycleSchedulePeriod: AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("MenuCycleSchedule")]
        public int MenuCycleScheduleId { get; set; }
        public virtual MenuCycleSchedule MenuCycleSchedule { get; set; }

        [ForeignKey("MealPeriod")]
        public int MealPeriodId { get; set; }
        public virtual MealPeriod MealPeriod { get; set; }

        public virtual ICollection<MenuCycleSchedulePeriodMenu> Menus { get; set; }
        public virtual ICollection<OutletMenuCycleSchedulePeriodMenu> OutletMenus { get; set; }
        public virtual ICollection<OutletMenuDish> OutletMenuDishes { get; set; }
    }
}
