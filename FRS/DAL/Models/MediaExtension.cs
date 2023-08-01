using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class MediaExtension : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Extension { get; set; }
        public string Label { get; set; }
        public int? min { get; set; }
        public int? max { get; set; }
    }
}
