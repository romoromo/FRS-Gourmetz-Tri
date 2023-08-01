using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class StudentGroupMealPlan : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Class")]
        public int StudentGroupId { get; set; }
        public virtual StudentGroup StudentGroup { get; set; }

        [ForeignKey("Dish")]
        public int DishId { get; set; }
        public virtual Dish Dish { get; set; }
    }
}
