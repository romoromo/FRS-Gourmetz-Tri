using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class RestrictionType : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }
        public string Label { get; set; }
        public string DietCondition { get; set; }
        public bool IsHandledEMR { get; set; }
        public bool IsDishTypeCustomisation { get; set; }
        public bool IsSpecialCondition { get; set; }
        public bool IsCarriedForward { get; set; }
        public bool IsMealFiltering { get; set; }
        public int Sequence { get; set; }
        public int? InstitutionId { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        public virtual ICollection<Restriction> Restrictions { get; set; }
    }
}
