using System.Collections.Generic;

namespace BAL.DTO.MealOrder
{
    public class ClassDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ClassLevelId { get; set; }

        public string ClassLevelName { get; set; }
        public List<ClassDetailDTO> Detail { get; set; }
    }

    public class ClassDetailDTO
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int? PeriodId { get; set; }
        public string PeriodName { get; set; }

        public int? SessionId { get; set; }
        public string SessionName { get; set; }
    }
}
