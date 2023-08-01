using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class LocationInstitution : AuditableEntity
    {
        [ForeignKey("Location")]
        public int LocationId { get; set; }
        public virtual Location Location { get; set; }

        public int InstitutionId { get; set; }
        public virtual Institution Institution { get; set; }

        public bool IsMain { get; set; }
    }
}
