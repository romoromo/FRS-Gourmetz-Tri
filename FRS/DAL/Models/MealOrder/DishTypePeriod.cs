using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DishTypePeriod
    {
        [ForeignKey("DishType")]
        public int DishTypeId { get; set; }
        public virtual DishType DishType { get; set; }

        [ForeignKey("Period")]
        public int PeriodId { get; set; }
        public virtual MealPeriod Period { get; set; }
    }
}
