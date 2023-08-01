using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class OutletProfile : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Label { get; set; }
        [ForeignKey("Caterer")]
        public int? CatererId { get; set; }
        public virtual CatererInfo Caterer { get; set; }

        public virtual List<Outlet> Outlets { get; set; }
        public virtual ICollection<MealPeriod> MealPeriods { get; set; }
        public virtual ICollection<MenuCycle> MenuCycles { get; set; }
    }
}
