using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class MealPeriod : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        public int? InstitutionId { get; set; }

        public int Sequence { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? OutletProfileId { get; set; }

        [ForeignKey("OutletProfileId")]
        public virtual OutletProfile OutletProfile { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        public virtual ICollection<DishTypePeriod> DishTypePeriods { get; set; }
        public virtual ICollection<DishCyclePeriod> DishCyclePeriods { get; set; }
    }
}
