using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class InterestGroupDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? InstitutionId { get; set; }
    }
}
