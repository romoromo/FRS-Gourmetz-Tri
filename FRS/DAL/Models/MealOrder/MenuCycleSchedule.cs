using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class MenuCycleSchedule: AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("MenuCycle")]
        public int MenuCycleId { get; set; }
        public virtual MenuCycle MenuCycle { get; set; }

        public int Day { get; set; }

        public virtual ICollection<MenuCycleSchedulePeriod> Periods { get; set; }
    }
}
