using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class Department : AuditableEntity
    {
        public Department()
        {
            this.Users = new HashSet<ApplicationUser>();
        }

        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Description { get; set; }

        [Sieve(CanFilter = true)]
        public int? InstitutionId { get; set; }

        public string AppName { get; set; }
        public virtual Institution Institution { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}
