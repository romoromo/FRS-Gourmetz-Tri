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

        public string Label { get; set; }
        public float Price { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int? MealTypeId { get; set; }

        [ForeignKey("StudentGroup")]
        public int StudentGroupId { get; set; }
        public virtual StudentGroup StudentGroup { get; set; }

        public int MealSessionId { get; set; }

        [ForeignKey("Dish")]
        public int DishId { get; set; }
        public virtual Dish Dish { get; set; }

        [ForeignKey("MealSessionDetail")]
        public int MealSessionDetailId { get; set; }
        public virtual MealSessionDetail MealSessionDetail { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreInfo Store { get; set; }
    }
}
