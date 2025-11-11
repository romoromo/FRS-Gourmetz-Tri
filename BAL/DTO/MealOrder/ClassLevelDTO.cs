using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class ClassLevelDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Year { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }

        public int? OutletId { get; set; }
        public string OutletName { get; set; }
        public int? MealSessionId { get; set; }
        public string MealSessionName { get; set; }
        public int? MealSessionDetailId { get; set; }
        public string MealSessionDetailName { get; set; }
        public List<ClassLevelDetailDTO> Detail { get; set; }
    }

    public class ClassLevelDetailDTO
    {
        public int Id { get; set; }
        public int ClassLevelId { get; set; }
        public int? PeriodId { get; set; }
        public string PeriodName { get; set; }

        public int? SessionId { get; set; }
        public string SessionName { get; set; }
    }
}
