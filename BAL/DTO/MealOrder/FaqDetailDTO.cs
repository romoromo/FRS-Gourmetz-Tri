using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class FaqDetailDTO
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public int FaqSubjectId { get; set; }
        public string FaqSubjectName { get; set; }
        public string FaqSubjectDescription { get; set; }
    }
}
