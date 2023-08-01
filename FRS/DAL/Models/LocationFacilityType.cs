using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class LocationFacilityType : AuditableEntity
    {
        [ForeignKey("Location")]
        public int LocationId { get; set; }
        public virtual Location Location { get; set; }

        [ForeignKey("FacilityType")]
        public int FacilityTypeId { get; set; }
        public virtual FacilityType FacilityType { get; set; }
    }
}
