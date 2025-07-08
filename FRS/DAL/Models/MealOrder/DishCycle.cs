using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DishCycle : AuditableEntity
    {
        public DishCycle()
        {
            this.Schedules = new List<DishCycleSchedule>();
        }

        [Key]
        public int Id { get; set; }

        public string Label { get; set; }

        public int Sequence { get; set; }
        [Sieve(CanFilter = true, CanSort = false)]
        public string CycleType { get; set; }

        public int? DishTypeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int NumOfDays { get; set; }

        public int NumOfSets { get; set; }

        public int? OutletProfileId { get; set; }

        public int? MealTypeId { get; set; }

        [ForeignKey("OutletProfileId")]
        public virtual OutletProfile OutletProfile { get; set; }

        [ForeignKey("DishTypeId")]
        public virtual DishType DishType { get; set; }

        [ForeignKey("MealTypeId")]
        public virtual MealType MealType { get; set; }

        public virtual ICollection<DishCycleSchedule> Schedules { get; set; }
        public virtual ICollection<DishCycleBlockedDate> BlockedDates { get; set; }
        public virtual ICollection<DishCycleScheduleSet> Sets { get; set; }
        public virtual ICollection<OutletDishBlockedDate> OutletDishBlockedDates { get; set; }
        public virtual ICollection<DishCyclePeriod> DishCyclePeriods { get; set; }
    }
}
