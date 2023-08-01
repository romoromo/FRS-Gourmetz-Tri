using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class ReservationPicture : AuditableEntity
    {

        [Key]
        public int Id { get; set; }
        public string PictureUrl { get; set; }
        public int? ReservationId { get; set; }

        [ForeignKey("ReservationId")]
        public virtual Reservation Reservation { get; set; }
    }
}
