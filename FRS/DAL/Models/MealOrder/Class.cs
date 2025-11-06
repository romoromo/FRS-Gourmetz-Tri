using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class Class : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        public int ClassLevelId { get; set; }

        [ForeignKey("ClassLevelId")]
        public virtual ClassLevel ClassLevel { get; set; }

        public virtual ICollection<OutletClassRosterSchedulePeriodClass> Periods { get; set; }
        public virtual ICollection<ClassDetail> ClassDetails { get; set; } = new HashSet<ClassDetail>();
    }

    public class ClassDetail
    {
        [Key]
        public int Id { get; set; }
        public int ClassId { get; set; }

        [ForeignKey(nameof(ClassId))]
        public virtual Class ClassLevel { get; set; }

        public int? PeriodId { get; set; }
        [ForeignKey(nameof(PeriodId))]
        public virtual MealSession MealPeriod { get; set; }

        public int? SessionId { get; set; }
        [ForeignKey(nameof(SessionId))]
        public virtual MealSessionDetail MealSession { get; set; }
    }
}
