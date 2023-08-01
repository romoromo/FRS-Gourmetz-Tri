using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class OutletDishCyclePeriodMenuDTO
    {
        public int DishId { get; set; }
        public int OutletId { get; set; }
        public string Label { get; set; }
        public int DishCyclePeriodId { get; set; }
        public int DishCycleScheduleSetId { get; set; }
        public DishDTO Dish { get; set; }
    }

    public class OutletDishViewMenuDTO
    {
        public int OutletId { get; set; }
        public List<OutletDishCyclePeriodMenuDTO> ExcludedMenus { get; set; }
    }
}
