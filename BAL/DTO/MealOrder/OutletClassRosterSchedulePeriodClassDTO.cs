using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class OutletClassRosterSchedulePeriodClassDTO
    {
        public int OutletClassRosterScheduleId { get; set; }
        public int MealSessionDetailId { get; set; }
        public int ClassId { get; set; }
        public string Name { get; set; }
        public int OutletClassRosterSchedulePeriodId { get; set; }
        public ClassDTO Class { get; set; }
    }
}
