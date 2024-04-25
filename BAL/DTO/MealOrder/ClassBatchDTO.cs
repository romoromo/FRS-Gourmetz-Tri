using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class ClassBatchDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Year { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }

        public int? OutletId { get; set; }
    }
}
