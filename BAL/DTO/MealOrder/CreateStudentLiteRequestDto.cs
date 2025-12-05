using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System;

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
        public DateTime CardIssueDate { get; set; }
        public string ParentEmail { get; set; }

        [Required]
        public int OutletId { get; set; }
        public int CurrentUserId { get; set; }

        public string Email { get; set; }

        public int? PhotoId { get; set; }
        public string PhotoName { get; set; }
        public string PhotoPath { get; set; }
        public string ImgUrl { get; set; }
    }
}
