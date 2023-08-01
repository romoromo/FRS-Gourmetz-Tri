using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class Module : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Route { get; set; }
        public string Uri { get; set; }

        public virtual ICollection<ModuleParameter> ModuleParameters { get; set; }
    }
}
