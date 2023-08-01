using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class MenuCycleScheduleDTO
    {
        public int Id { get; set; }

        public int MenuCycleId { get; set; }

        public int Day { get; set; }

        public List<MenuCycleSchedulePeriodDTO> Periods { get; set; }
    }
}
