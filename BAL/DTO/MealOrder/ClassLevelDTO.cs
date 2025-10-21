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
    }
}
