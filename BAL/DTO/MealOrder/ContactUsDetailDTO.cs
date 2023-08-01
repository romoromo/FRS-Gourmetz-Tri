using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class ContactUsDetailDTO
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public int ContactUsSubjectId { get; set; }
        public string ContactUsSubjectName { get; set; }
        public string ContactUsSubjectDescription { get; set; }
    }
}
