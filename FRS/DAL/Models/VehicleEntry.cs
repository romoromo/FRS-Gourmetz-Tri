using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class VehicleEntry : AuditableEntity
    {
        public VehicleEntry()
        {
        }

        [Key]
        public int Id { get; set; }
        public string SeasonId { get; set; }
        public string PersonName { get; set; }
        public string CardType { get; set; }
        public string Vehicle { get; set; }
        public string VehicleStatus { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        
        public string Type { get; set; }
        public DateTime? Timestamp { get; set; }
        public int? ReservationId { get; set; }

        [ForeignKey("ReservationId")]
        public virtual Reservation Reservation { get; set; }
    }
}
