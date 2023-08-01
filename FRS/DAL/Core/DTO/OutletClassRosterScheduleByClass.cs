using DAL.Models.MealOrder;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class OutletClassRosterScheduleByClass
    {
        public int Id { get; set; }

        public int OutletClassRosterId { get; set; }
        public OutletClassRoster OutletClassRoster { get; set; }

        public int Day { get; set; }

        public ICollection<OutletClassRosterSchedulePeriod> Periods { get; set; }
    }
}
