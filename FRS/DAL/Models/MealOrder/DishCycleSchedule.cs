using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DishCycleSchedule: AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("DishCycle")]
        public int DishCycleId { get; set; }
        public virtual DishCycle DishCycle { get; set; }

        public int Day { get; set; }

        public virtual ICollection<DishCycleScheduleDetail> Details { get; set; }
    }
}
