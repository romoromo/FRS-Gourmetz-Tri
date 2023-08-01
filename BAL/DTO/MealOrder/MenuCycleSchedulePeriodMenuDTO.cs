using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class MenuCycleSchedulePeriodMenuDTO
    {
        public int MenuCycleScheduleId { get; set; }
        public int MenuId { get; set; }
        public string Label { get; set; }
        public string Code { get; set; }
        public MenuDTO Menu { get; set; }
    }
}
