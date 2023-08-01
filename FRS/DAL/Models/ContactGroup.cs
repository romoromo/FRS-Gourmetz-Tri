using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class ContactGroup : AuditableEntity
    {
        public ContactGroup()
        {
            this.Members = new HashSet<ContactGroupMember>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? InstitutionId { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        public virtual ICollection<ContactGroupDepartment> ContactGroupDepartments { get; set; }
        public virtual ICollection<ContactGroupMember> Members { get; set; }
    }
}
