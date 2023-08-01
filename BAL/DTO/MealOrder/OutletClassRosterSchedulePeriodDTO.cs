using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class OutletClassRosterSchedulePeriodDTO
    {
        public int Id { get; set; }
        public int OutletClassRosterScheduleId { get; set; }
        public int? MealSessionDetailId { get; set; }
        public string MealSessionDetailName { get; set; }
        public DateTime MealSessionDetailStartDate { get; set; }
        public DateTime? MealSessionDetailEndDate { get; set; }

        public DateTime OutletClassRosterStartDate { get; set; }
        public DateTime OutletClassRosterEndDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public float RouteInterval { get; set; }
        public float OverheadInterval { get; set; }

        public List<OutletClassRosterSchedulePeriodClassDTO> Classes { get; set; }
    }
}
