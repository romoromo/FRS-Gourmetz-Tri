
using System;
using System.Collections.Generic;

namespace FRS.ViewModels
{
    public class EmployeeScheduleSlotViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string CoverName { get; set; }

        public int? ScheduleId { get; set; }

        public int? EmployeeDataId { get; set; }

        public int? CoveringEmployeeDataId { get; set; }

        public int? LocationId { get; set; }

        public int? ShiftId { get; set; }

        public int? day { get; set; }

        public int IsActive { get; set; }
    }
}
