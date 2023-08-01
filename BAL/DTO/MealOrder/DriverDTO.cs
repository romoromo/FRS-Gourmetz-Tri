using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class DriverDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Contact { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }
    }
}
