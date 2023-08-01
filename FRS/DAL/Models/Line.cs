using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class Line : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }

        public int? MapId { get; set; }
        [ForeignKey("MapId")]
        public virtual Map Map { get; set; }

        public int? Point0Id { get; set; }
        [ForeignKey("Point0Id")]
        public virtual Point Point0 { get; set; }

        public int? Point1Id { get; set; }
        [ForeignKey("Point1Id")]
        public virtual Point Point1 { get; set; }

        public string Code0 { get; set; }
        public string Code1 { get; set; }
        public bool isFloorConnector { get; set; }
    }
}
