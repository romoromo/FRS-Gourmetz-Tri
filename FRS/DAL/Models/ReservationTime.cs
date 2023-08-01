using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class ReservationTime : AuditableEntity
    {
        [ForeignKey("TimeInterval")]
        public int TimeIntervalId { get; set; }
        public virtual TimeInterval TimeInterval { get; set; }

        [ForeignKey("Reservation")]
        public int ReservationId { get; set; }
        public virtual Reservation Reservation { get; set; }
    }
}
