using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class OutletClassRosterDTO
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? OutletProfileId { get; set; }
        public string OutletProfileName { get; set; }
        public int? MealSessionId { get; set; }

        public List<OutletClassRosterScheduleDTO> Schedules { get; set; }
        public List<int> Days { get; set; }
    }

    public class OutletClassRosterScheduleByDay
    {
        public int Day { get; set; }

        public List<OutletClassRosterScheduleDTO> Schedules { get; set; }
        public List<OutletClassRosterSchedulePeriodDTO> Periods { get; set; }
    }
}
