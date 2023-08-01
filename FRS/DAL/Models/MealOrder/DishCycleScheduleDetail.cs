using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DishCycleScheduleDetail: AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("DishCycleSchedule")]
        public int DishCycleScheduleId { get; set; }
        public virtual DishCycleSchedule DishCycleSchedule { get; set; }

        public string Label { get; set; }

        public int Sequence { get; set; }

        [ForeignKey("DishCycle")]
        public int? DishCycleId { get; set; }
        public virtual DishCycle DishCycle { get; set; }

        public virtual ICollection<DishCycleScheduleDetailMenu> Menus { get; set; }
    }
}
