using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class MealAllocation : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime? deliveryDate { get; set; }
        public DateTime? PackingTime { get; set; }
        public int? periodId { get; set; }
        [ForeignKey("periodId")]
        public virtual MealPeriod Period { get; set; } 
        public int? mealSessionId { get; set; }
        [ForeignKey("mealSessionId")]
        public virtual MealSessionDetail MealSessionDetail { get; set; }
        public int? outletId { get; set; }
        [ForeignKey("outletId")]
        public virtual Outlet Outlet { get; set; }
        public virtual ICollection<TokenLabel> tokens { get; set; }
    }
}
