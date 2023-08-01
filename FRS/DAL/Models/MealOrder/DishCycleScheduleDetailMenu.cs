using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DishCycleScheduleDetailMenu: AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("DishCycleScheduleDetail")]
        public int DishCycleScheduleDetailId { get; set; }
        public virtual DishCycleScheduleDetail DishCycleScheduleDetail { get; set; }

        [ForeignKey("Dish")]
        public int? DishId { get; set; }

        public virtual Dish Dish { get; set; }

        [ForeignKey("DishCycle")]
        public int? DishCycleId { get; set; }

        public virtual DishCycle DishCycle { get; set; }
    }
}
