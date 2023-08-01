using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class MealPeriodDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Sequence { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }

        public int? OutletProfileId { get; set; }
        public string OutletProfileName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
