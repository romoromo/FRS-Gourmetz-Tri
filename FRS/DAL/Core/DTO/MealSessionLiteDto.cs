using System.Collections.Generic;

namespace DAL.Core.DTO
{
    public class MealSessionLiteDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Sequence { get; set; }
        public List<MealSessionDetailLiteDto> Details { get; set; } = new List<MealSessionDetailLiteDto>();
    }

    public class MealSessionDetailLiteDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
