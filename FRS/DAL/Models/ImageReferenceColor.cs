using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class ImageReferenceColor : AuditableEntity
    {
        public ImageReferenceColor()
        {
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string ColorCode { get; set; }

        public int? InstitutionId { get; set; }

        public virtual Institution Institution { get; set; }
    }
}
