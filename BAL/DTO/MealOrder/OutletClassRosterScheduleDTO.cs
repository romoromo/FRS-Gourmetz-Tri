using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class OutletClassRosterScheduleDTO
    {
        public int Id { get; set; }

        public int OutletClassRosterId { get; set; }

        public OutletClassRosterDTO OutletClassRoster { get; set; }

        public int Day { get; set; }

        public List<OutletClassRosterSchedulePeriodDTO> Periods { get; set; }

        public bool HasClass { get; set; }
    }
}
