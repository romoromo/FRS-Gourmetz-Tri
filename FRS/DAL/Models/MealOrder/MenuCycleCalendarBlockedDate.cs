using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class MenuCycleCalendarBlockedDate : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public string Label { get; set; }

        public int? MenuCycleCalendarId { get; set; }

        public DateTime EffectiveDate { get; set; }

        [ForeignKey("MenuCycleCalendarId")]
        public virtual MenuCycleCalendar MenuCycleCalendar { get; set; }
    }
}
