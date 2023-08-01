using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class ReservationInvitee : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        [ForeignKey("User")]
        public int? UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("Reservation")]
        public int ReservationId { get; set; }
        public virtual Reservation Reservation { get; set; }

        public string Status { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Designation { get; set; }
        public DateTime? AttendedOn { get; set; }
        public string Department { get; set; }
        public string PhoneNumber { get; set; }


        public string CardType { get; set; }
        public string PlateNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string VehicleStatus { get; set; }

        public DateTime? EntryDate { get; set; }
        public DateTime? ExitDate { get; set; }

        [ForeignKey("ContactGroup")]
        public int? ContactGroupId { get; set; }
        public virtual ContactGroup ContactGroup { get; set; }
    }
}
