using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class OutletMenuCycleSchedulePeriodDTO
    {
        public int MenuCycleScheduleId { get; set; }
        public int? MealPeriodId { get; set; }
        public string MealPeriodName { get; set; }
        public int OutletId { get; set; }
        public List<OutletMenuCycleSchedulePeriodMenuDTO> Menus { get; set; }
        public List<OutletMenuDishDTO> OutletMenuDishes { get; set; }
    }
}
