using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class TokenAltDish : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int? TokenOrderedId { get; set; }
        [ForeignKey("TokenOrderedId")]
        public virtual TokenOrdered TokenOrdered { get; set; }
        public int? DishId { get; set; }
        [ForeignKey("DishId")]
        public virtual Dish Dish { get; set; }
    }
}
