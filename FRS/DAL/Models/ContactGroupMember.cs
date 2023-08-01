using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class ContactGroupMember : AuditableEntity
    {
        public ContactGroupMember()
        {
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Company { get; set; }
        public string PhoneNumber { get; set; }
        public string HomeNo { get; set; }
        public string MobileNo { get; set; }

        public int ContactGroupId { get; set; }

        public virtual ContactGroup ContactGroup { get; set; }

        public int? FileId { get; set; }

        [ForeignKey("FileId")]
        public virtual File Icon { get; set; }

        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
