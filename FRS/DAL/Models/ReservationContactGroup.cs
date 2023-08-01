using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class ReservationContactGroup : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        [ForeignKey("ContactGroup")]
        public int? ContactGroupId { get; set; }
        public virtual ContactGroup ContactGroup { get; set; }

        [ForeignKey("Reservation")]
        public int ReservationId { get; set; }
        public virtual Reservation Reservation { get; set; }
    }
}
