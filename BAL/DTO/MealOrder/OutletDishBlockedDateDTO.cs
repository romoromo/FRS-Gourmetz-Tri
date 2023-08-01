using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class OutletDishBlockedDateDTO
    {
        public int Id { get; set; }

        public string Label { get; set; }

        public int? DishCycleId { get; set; }

        public int? OutletId { get; set; }

        public DateTime EffectiveDate { get; set; }
    }
}
