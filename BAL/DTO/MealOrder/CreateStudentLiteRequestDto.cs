using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BAL.DTO.MealOrder
{
    public class CreateStudentLiteRequestDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public int ClassId { get; set; }
        [Required]
        public int ClassLevelId { get; set; }
        [Required]
        public int ClassBatchId { get; set; }
        public List<int> RestrictionsIds { get; set; } = new List<int>();
        public string CardId { get; set; }
        [Required]
        public int OutletId { get; set; }
    }
}
