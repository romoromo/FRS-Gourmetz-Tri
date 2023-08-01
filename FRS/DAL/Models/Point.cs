using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class Point : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }

        public int x { get; set; }
        public int y { get; set; }

        public bool IsFloorConnector { get; set; }

        public bool NoWheelchair { get; set; }
        public bool NoSheltered { get; set; }

        public int? MapId { get; set; }
        [ForeignKey("MapId")]
        public virtual Map Map { get; set; }

        public virtual ICollection<PointDirectoryListing> Directorys { get; set; }
        
    }
}
