using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class MealSessionDetailDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Sequence { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int MealSessionId { get; set; }

        public string MealSessionName { get; set; }
        public bool IsActive { get; set; }

        public DateTime? RouteTime { get; set; }
        public DateTime? OverheadTime { get; set; }
        public DateTime? CalSourceTime { get; set; }
        public int? RouteId { get; set; }
        public string RouteName { get; set; }
        public string RouteColor { get; set; }
        public string STime { get; set; }
        public string RTime { get; set; }
        public string OTime { get; set; }
        public string CTime { get; set; }
        public float RouteInterval { get; set; }
        public float OverheadInterval { get; set; }
        public int? MealPeriodId { get; set; }
        public string MealPeriodName { get; set; }
    }
}
