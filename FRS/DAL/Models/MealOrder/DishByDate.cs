using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DishByDate
    {
        public DateTime Date { get; set; }
        public List<DishCycle> DishCycles { get; set; }
        public List<DishCycleScheduleSet> DishSets { get; set; }
        public List<DishCycleScheduleDetailMenu> Menus { get; set; }
    }
}
