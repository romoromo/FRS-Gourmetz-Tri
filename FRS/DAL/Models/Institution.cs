using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class Institution : AuditableEntity
    {
        public Institution()
        {
            this.Users = new HashSet<ApplicationUser>();
            this.Departments = new HashSet<Department>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public bool IsRestrictDuplicateBooking { get; set; }
        public string Restriction { get; set; }
        public bool IsDefault { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
    }
}
