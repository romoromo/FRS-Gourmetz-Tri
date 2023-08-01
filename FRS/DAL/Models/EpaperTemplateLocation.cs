using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class EpaperTemplateLocation : AuditableEntity
    {
        [ForeignKey("Location")]
        public int LocationId { get; set; }
        public virtual Location Location { get; set; }

        [ForeignKey("EpaperTemplate")]
        public int EpaperTemplateId { get; set; }
        public virtual EpaperTemplate EpaperTemplate { get; set; }
    }
}
