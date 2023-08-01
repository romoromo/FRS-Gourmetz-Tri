using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DishPeriod
    {
        [ForeignKey("Dish")]
        public int DishId { get; set; }
        public virtual Dish Dish { get; set; }

        [ForeignKey("Period")]
        public int PeriodId { get; set; }
        public virtual MealPeriod Period { get; set; }
    }
}
