using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class SignageCompilationComponent : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int? ComponentId { get; set; }
        [ForeignKey("ComponentId")]
        public virtual SignageComponent Component { get; set; }

        public int? CompilationId { get; set; }
        [ForeignKey("CompilationId")]
        public virtual SignageCompilation Compilation { get; set; }

        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int order { get; set; }
    }
}
