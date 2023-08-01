using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class Map : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }

        public int? FloorId { get; set; }
        [ForeignKey("FloorId")]
        public virtual Floor Floor { get; set; }

        public string map_url { get; set; }

        public virtual ICollection<Point> Points { get; set; }
        public virtual ICollection<Line> Lines { get; set; }
    }
}
