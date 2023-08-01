using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class OutletClassRosterSchedulePeriodClass : AuditableEntity
    {
        [ForeignKey("OutletClassRosterSchedulePeriod")]
        public int OutletClassRosterSchedulePeriodId { get; set; }
        public virtual OutletClassRosterSchedulePeriod OutletClassRosterSchedulePeriod { get; set; }

        [ForeignKey("Class")]
        public int ClassId { get; set; }
        public virtual Class Class { get; set; }
    }
}
