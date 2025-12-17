using DAL.Core;
using System.Collections.Generic;

namespace BAL.DTO.MealOrder
{
    public class ClassLevelScheduleDTO
    {
        public int Id { get; set; }
        public int ClassLevelId { get; set; }
        public List<ScheduleItemDTO> Schedules { get; set; } = [];
    }

    public class ScheduleItemDTO
    {
        public int PeriodId { get; set; }
        public ScheduleDay Day { get; set; }
        public int SessionId { get; set; }
    }
}
