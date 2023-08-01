using DAL.Models.MealOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Core.DTO
{
    public class DishCycleScheduleSetMenuDTO
    {
        public int Id { get; set; }

        public int Sequence { get; set; }
        public string Label { get; set; }
        public string CycleType { get; set; }
        public int? CycleTypeId { get; set; }
        public int? CycleTypeSequence { get; set; }
        public int DishCycleId { get; set; }
        public DishCycle DishCycle { get; set; }
        public DishCycle DishCycleType { get; set; }
        public MealType MealType { get; set; }
        public List<DishCycleScheduleDetailMenu> Menus { get; set; }
        public List<OutletDishCyclePeriodMenu> ExcludedMenus { get; set; }
    }

    public class MealCreditSetMenuDTO
    {
        public int MealTypeId { get; set; }
        public int DishCycleId { get; set; }
        public string Setname {
            get
            {
                return string.Join(" + ", Menus.Select(e => e.Dish.Label));
            }
        }
        public List<DishCycleScheduleDetailMenu> Menus { get; set; }
        public List<OutletDishCyclePeriodMenu> ExcludedMenus { get; set; }
    }
}
