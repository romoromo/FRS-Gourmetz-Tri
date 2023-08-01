using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class Restriction : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }
        public string Label { get; set; }
        public int RestrictionTypeId { get; set; }
        public int Sequence { get; set; }
        public bool IsHighPriority { get; set; }
        public bool IsIncludeHighPriorityFiltering { get; set; }
        public bool IsAvailableDateSpecific { get; set; }
        public bool IsMealFiltering { get; set; }

        [ForeignKey("RestrictionTypeId")]
        public virtual RestrictionType RestrictionType { get; set; }
    }
}
