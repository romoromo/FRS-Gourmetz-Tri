using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class MenuCycleDTO
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? InstitutionId { get; set; }
        public string InstitutioName { get; set; }
        public int? OutletProfileId { get; set; }
        public string OutletProfileName { get; set; }

        public List<MenuCycleScheduleDTO> Schedules { get; set; }
        public List<MenuCycleBlockedDateDTO> BlockedDates { get; set; }
        public List<OutletBlockedDateDTO> OutletBlockedDates { get; set; }
        public List<int> Days { get; set; }
    }
}
