using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class DishCyclePeriodDTO
    {
        public int Id { get; set; }
        public int DishCycleId { get; set; }
        public int MealPeriodId { get; set; }
        public string MealPeriodName { get; set; }

        public List<OutletDishCyclePeriodMenuDTO> ExcludedMenus { get; set; }
    }
}
