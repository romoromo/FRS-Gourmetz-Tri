using System;

namespace BAL.DTO.MealOrder
{
    public class OutletMealSessionInput
    {
        public int OutletId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? OrderDateTo { get; set; }
    }
}
