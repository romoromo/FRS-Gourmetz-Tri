using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class MenuCycle : AuditableEntity
    {
        public MenuCycle()
        {
            this.Schedules = new List<MenuCycleSchedule>();
        }

        [Key]
        public int Id { get; set; }

        public string Label { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? InstitutionId { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        public int? OutletProfileId { get; set; }

        [ForeignKey("OutletProfileId")]
        public virtual OutletProfile OutletProfile { get; set; }

        public virtual ICollection<MenuCycleSchedule> Schedules { get; set; }

        public virtual ICollection<MenuCycleBlockedDate> BlockedDates { get; set; }

        public virtual ICollection<OutletBlockedDate> OutletBlockedDates { get; set; }
    }
}
