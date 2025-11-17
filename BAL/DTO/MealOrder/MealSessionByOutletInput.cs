using System;

namespace BAL.DTO.MealOrder
{
    public class MealSessionByOutletInput
    {
        public int OutletId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? OrderDateTo { get; set; }

        public int ClassLevelId { get; set; }
    }

    public class MealSessionByStudentGrouptInput : MealSessionByOutletInput
    {
        public int StudentGroupId { get; set; }

    }
}
