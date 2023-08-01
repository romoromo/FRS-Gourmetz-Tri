using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class SignageCompilation : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int width { get; set; }
        public int height { get; set; }

        public string BackgroundColor { get; set; }
        public string BackgroundImage { get; set; }

        public virtual ICollection<SignageCompilationComponent> Components { get; set; }
    }
}
