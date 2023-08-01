using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class UserGroup : AuditableEntity
    {
        public UserGroup()
        {
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int InstitutionId { get; set; }

        public string emails { get; set; }

        public virtual ICollection<UserGroupMember> Members { get; set; }
        public virtual ICollection<UserGroupLocation> Locations { get; set; }
        public virtual Institution Institution { get; set; }
    }
}
