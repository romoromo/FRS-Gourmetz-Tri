using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class OutletClassRoster : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public string Label { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? OutletProfileId { get; set; }

        [ForeignKey("OutletProfileId")]
        public virtual OutletProfile OutletProfile { get; set; }

        public int? MealSessionId { get; set; }

        [ForeignKey("MealSessionId")]
        public virtual MealSession MealSession { get; set; }

        public virtual ICollection<OutletClassRosterSchedule> Schedules { get; set; }
    }
}
