using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class PackingAllocation : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime? PackingDate { get; set; }
        public int? RouteId { get; set; }
        [ForeignKey("RouteId")]
        public virtual Route Route { get; set; } 
        public int? OutletId { get; set; }
        [ForeignKey("OutletId")]
        public virtual Outlet Outlet { get; set; }
        public virtual ICollection<DishAllocation> Dishes { get; set; }
    }
}
