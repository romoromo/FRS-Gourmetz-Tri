using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class RestrictionDTO
    {
        public int Id { get; set; }

        public string Code { get; set; }
        public string Label { get; set; }
        public int RestrictionTypeId { get; set; }
        public string RestrictionTypeLabel { get; set; }
        public string RestrictionTypeCode { get; set; }
        public int Sequence { get; set; }
        public bool IsHighPriority { get; set; }
        public bool IsIncludeHighPriorityFiltering { get; set; }
        public bool IsAvailableDateSpecific { get; set; }
        public bool IsMealFiltering { get; set; }

    }
}
