using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class DishTypeDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }

        public int? CatererId { get; set; }

        public string DishTypePeriodNames { get { return DishTypePeriods != null ? string.Join(",", DishTypePeriods.Select(e => e.PeriodName)) : string.Empty; } }

        public List<DishTypePeriodDTO> DishTypePeriods { get; set; }
        public int? OrderNumber { get; set; }
    }
}
