using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class BuildingFloor : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int? BuildingId { get; set; }
        [ForeignKey("BuildingId")]
        public virtual Building Building { get; set; }

        public int? FloorId { get; set; }
        [ForeignKey("FloorId")]
        public virtual Floor Floor { get; set; }

        public int order { get; set; }
    }
}
