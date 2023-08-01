using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class MenuCycleSchedulePeriodDTO
    {
        public int MenuCycleScheduleId { get; set; }
        public int? MealPeriodId { get; set; }
        public string MealPeriodName { get; set; }
        public ICollection<MenuCycleSchedulePeriodMenuDTO> Menus { get; set; }
        public ICollection<OutletMenuCycleSchedulePeriodMenuDTO> OutletMenus { get; set; }
        public ICollection<OutletMenuDishDTO> OutletMenuDishes { get; set; }
    }
}
