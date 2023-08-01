using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class StoreInfoDTO
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string StoreType { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }

        public int? CatererInfoId { get; set; }

        public string CatererInfoName { get; set; }

        public string CutoffTime { get; set; }
        public string CalendarDays { get; set; }

        public int? OutletId { get; set; }
    }
}
