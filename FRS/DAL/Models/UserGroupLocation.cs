using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class UserGroupLocation : AuditableEntity
    {
        public UserGroupLocation()
        {
        }

        [Key]
        public int Id { get; set; }
        public int UserGroupId { get; set; }
        public int LocationId { get; set; }

        [ForeignKey("UserGroupId")]
        public virtual UserGroup UserGroup { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }
    }
}
