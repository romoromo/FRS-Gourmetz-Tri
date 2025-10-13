using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class CuisineDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int? FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }
    }
}
