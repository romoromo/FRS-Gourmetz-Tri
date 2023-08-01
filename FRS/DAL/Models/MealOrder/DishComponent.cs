using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DishComponent : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public string Category { get; set; }
        public string Component { get; set; }
        public float Weight { get; set; }
        public int Quantity { get; set; }
        public string SapProductCode { get; set; }

        public int DishId { get; set; }
        [ForeignKey("DishId")]
        public virtual Dish Dish { get; set; }
    }
}
