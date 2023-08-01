using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class DishAllocation : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public int? PackingId { get; set; }
        [ForeignKey("PackingId")]
        public virtual PackingAllocation Packing { get; set; } 
        public int? DishId { get; set; }
        [ForeignKey("DishId")]
        public virtual Dish Dish { get; set; }
        public int? Qty { get; set; }
    }
}
