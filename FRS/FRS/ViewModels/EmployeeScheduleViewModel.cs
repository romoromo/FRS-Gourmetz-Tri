using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class EmployeeScheduleViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? IneffectiveDate { get; set; }

        public List<EmployeeScheduleShiftViewModel> shifts { get; set; }
        public List<EmployeeScheduleSlotViewModel> slots { get; set; }
        public List<EmployeeScheduleInfoViewModel> infos { get; set; }
        public List<LocationViewModel> locations { get; set; }
    }
}
