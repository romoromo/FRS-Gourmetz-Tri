using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DishCycleScheduleSet : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int Sequence { get; set; }
        public string Label { get; set; }
        public string CycleType { get; set; }

        [ForeignKey("DishCycleType")]
        public int? CycleTypeId { get; set; }
        [ForeignKey("DishCycle")]
        public int DishCycleId { get; set; }

        public int? CycleTypeSequence { get; set; }

        [ForeignKey("MealType")]
        public int? MealTypeId { get; set; }

        public double Price { get; set; }

        public virtual DishCycle DishCycle { get; set; }
        public virtual DishCycle DishCycleType { get; set; }
        public virtual MealType MealType { get; set; }
    }
}
