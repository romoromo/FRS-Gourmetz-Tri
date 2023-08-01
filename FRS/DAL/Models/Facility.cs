using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class Facility : AuditableEntity
    {
        public Facility()
        {
            this.LocationFacilities = new HashSet<LocationFacility>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public int? InstitutionId { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        public int? FileId { get; set; }

        [ForeignKey("FileId")]
        public virtual File Icon { get; set; }

        public virtual ICollection<LocationFacility> LocationFacilities { get; set; }
    }
}
