using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class MenuGroupDishCycleDTO
    {
        public int Id { get; set; }
        public int DishCycleId { get; set; }
        public string DishCycleLabel { get; set; }
        public int MenuGroupId { get; set; }
        public string MenuGroupName { get; set; }

        public DishCycleDTO DishCycle { get; set; }
    }
}
