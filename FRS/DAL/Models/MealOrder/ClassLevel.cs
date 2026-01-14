using DAL.Core;
using NPOI.DDF;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class ClassLevel : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        public int Year { get; set; }

        public int? InstitutionId { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        public int? OutletId { get; set; }

        [ForeignKey("OutletId")]
        public virtual Outlet Outlet { get; set; }

        public int? MealSessionId { get; set; }

        [ForeignKey("MealSessionId")]
        public virtual MealSession MealSession { get; set; }

        public int? MealSessionDetailId { get; set; }

        [ForeignKey("MealSessionDetailId")]
        public virtual MealSessionDetail MealSessionDetail { get; set; }

        public virtual ICollection<ClassLevelDetail> ClassLevelDetails { get; set; } = new HashSet<ClassLevelDetail>();

        public bool? FASRechargeable { get; set; }
        public ScheduleDay? Day { get; set; }
        public TimeSpan? Time { get; set; }
        public double? Amount { get; set; }
    }

    public class ClassLevelDetail
    {
        [Key]
        public int Id { get; set; }
        public int ClassLevelId { get; set; }

        [ForeignKey(nameof(ClassLevelId))]
        public virtual ClassLevel ClassLevel { get; set; }

        public int? PeriodId { get; set; }
        [ForeignKey(nameof(PeriodId))]
        public virtual MealSession MealPeriod { get; set; }

        public int? SessionId { get; set; }
        [ForeignKey(nameof(SessionId))]
        public virtual MealSessionDetail MealSession { get; set; }
    }

    public class ClassLevelSchedule
    {
        [Key]
        public int Id { get; set; }

        public int ClassLevelId { get; set; }

        [ForeignKey(nameof(ClassLevelId))]
        public virtual ClassLevel ClassLevel { get; set; }

        public virtual ICollection<ClassLevelScheduleItem> Schedules { get; set; }
            = [];
    }

    public class ClassLevelScheduleItem
    {
        [Key]
        public int Id { get; set; }

        public int? ClassLevelScheduleId { get; set; }
        public virtual ClassLevelSchedule ClassLevelSchedule { get; set; }

        public int? PeriodId { get; set; }
        [ForeignKey(nameof(PeriodId))]
        public virtual MealSession MealPeriod { get; set; }

        public int? SessionId { get; set; }
        [ForeignKey(nameof(SessionId))]
        public virtual MealSessionDetail MealSession { get; set; }

        public ScheduleDay Day { get; set; }
    }
}
