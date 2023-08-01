using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class MediaUserGroup : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int? MediaId { get; set; }
        public int? UserGroupId { get; set; }

        [ForeignKey("MediaId")]
        public virtual Media Media { get; set; }

        [ForeignKey("UserGroupId")]
        public virtual UserGroup UserGroup { get; set; }
    }
}
