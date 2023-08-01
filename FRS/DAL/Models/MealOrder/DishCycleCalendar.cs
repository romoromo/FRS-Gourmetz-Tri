using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DishCycleCalendar : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public string Label { get; set; }

        public int? DishCycleId { get; set; }

        [ForeignKey("DishCycleId")]
        public virtual DishCycle DishCycle { get; set; }

        public virtual List<MenuCycleCalendarBlockedDate> BlockedDates { get; set; }
    }
}
