using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class ContactGroupDepartment : AuditableEntity
    {
        [ForeignKey("ContactGroup")]
        public int ContactGroupId { get; set; }
        public virtual ContactGroup ContactGroup { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
    }
}
