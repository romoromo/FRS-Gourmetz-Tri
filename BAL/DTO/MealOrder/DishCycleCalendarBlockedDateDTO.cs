using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class DishCycleCalendarBlockedDateDTO
    {
        public int Id { get; set; }

        public string Label { get; set; }

        public int? DishCycleCalendarId { get; set; }

        public DateTime EffectiveDate { get; set; }
    }
}
