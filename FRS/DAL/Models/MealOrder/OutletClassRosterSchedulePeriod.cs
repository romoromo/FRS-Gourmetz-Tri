using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class OutletClassRosterSchedulePeriod : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("OutletClassRosterSchedule")]
        public int OutletClassRosterScheduleId { get; set; }
        public virtual OutletClassRosterSchedule OutletClassRosterSchedule { get; set; }

        [ForeignKey("MealSessionDetail")]
        public int MealSessionDetailId { get; set; }
        public virtual MealSessionDetail MealSessionDetail { get; set; }

        public virtual ICollection<OutletClassRosterSchedulePeriodClass> Classes { get; set; }
    }
}
