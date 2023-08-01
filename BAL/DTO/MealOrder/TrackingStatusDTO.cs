using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class TrackingStatusDTO
    {
        public int Id { get; set; }

        public string Status { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }
    }
}
