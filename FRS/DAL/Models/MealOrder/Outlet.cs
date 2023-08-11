using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class Outlet : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }

        //[ForeignKey("OutletProfile")]
        //public int? OutletProfileId { get; set; }
        //public virtual OutletProfile OutletProfile { get; set; }
        public int DaysToFreezeOrdering { get; set; }
        public int? LocationId { get; set; }
        public virtual Location Location { get; set; }

        public virtual List<CatererOutlet> CatererOutlets { get; set; }

        public virtual ICollection<ClassLevel> ClassLevels { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<MealSession> MealSessions { get; set; }
        public virtual ICollection<OutletMenuCycleSchedulePeriodMenu> OutletMenuCycleSchedulePeriodMenus { get; set; }
        public virtual ICollection<MenuCycle> MenuCycles { get; set; }
        public virtual ICollection<OutletTerm> Terms { get; set; }

    }
}
