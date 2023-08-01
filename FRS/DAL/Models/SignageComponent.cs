using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class SignageComponent : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int? FixWidth { get; set; }
        public int? FixHeight { get; set; }
        public bool IsRatio { get; set; }

        public string BackgroundColor { get; set; }
        public string BackgroundImage { get; set; }

        public string ComponentType { get; set; }
        
        public string Configurations { get; set; }

        public int? PreviewWidth { get; set; }
        public int? PreviewHeight { get; set; }
    }
}
