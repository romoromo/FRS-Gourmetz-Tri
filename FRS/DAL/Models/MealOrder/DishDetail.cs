using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DishDetail : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public int ParentDishId { get; set; }
        [ForeignKey("ParentDishId")]
        public virtual Dish ParentDish { get; set; }

        public int? DishId { get; set; }
        public float CookedWeight { get; set; }
        [ForeignKey("DishId")]
        public virtual Dish Dish { get; set; }
    }
}
