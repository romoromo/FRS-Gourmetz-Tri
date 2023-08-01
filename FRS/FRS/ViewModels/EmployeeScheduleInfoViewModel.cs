
using System;
using System.Collections.Generic;

namespace FRS.ViewModels
{
    public class EmployeeScheduleInfoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? ScheduleId { get; set; }

        public int? LocationId { get; set; }

        public int? ShiftId { get; set; }

        public int? day { get; set; }

        public int IsActive { get; set; }

        public bool extend { get; set; }
        public bool noDisplay { get; set; }
    }
}
