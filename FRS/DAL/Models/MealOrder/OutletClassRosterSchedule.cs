using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class OutletClassRosterSchedule : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("OutletClassRoster")]
        public int OutletClassRosterId { get; set; }
        public virtual OutletClassRoster OutletClassRoster { get; set; }

        public int Day { get; set; }

        public virtual ICollection<OutletClassRosterSchedulePeriod> Periods { get; set; }
    }
}
