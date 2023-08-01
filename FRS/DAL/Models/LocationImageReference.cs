using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class LocationImageReference : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Location")]
        public int LocationId { get; set; }
        public virtual Location Location { get; set; }

        [ForeignKey("File")]
        public int? FileId { get; set; }
        public virtual File File { get; set; }

        public string Remarks { get; set; }

        [ForeignKey("ImageReferenceType")]
        public int? ImageReferenceTypeId { get; set; }
        public virtual ImageReferenceType ImageReferenceType { get; set; }

        [ForeignKey("ImageReferenceColor")]
        public int? ImageReferenceColorId { get; set; }
        public virtual ImageReferenceColor ImageReferenceColor { get; set; }

        public DateTime? ReferenceDate { get; set; }

        public string ColorCode { get; set; }
    }
}
