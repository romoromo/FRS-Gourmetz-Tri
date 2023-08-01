using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class MediaRole : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int? MediaId { get; set; }
        public int? RoleId { get; set; }

        [ForeignKey("MediaId")]
        public virtual Media Media { get; set; }

        [ForeignKey("RoleId")]
        public virtual ApplicationRole Role { get; set; }
    }
}
