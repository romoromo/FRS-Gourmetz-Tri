using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class ImageFile : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        //public string ImageName { get; set; }
        public string ImageLocation { get; set; }
        public int? InstitutionId { get; set; }
        public bool IsVideo { get; set; }

        public int? MediaId { get; set; }
        [ForeignKey("MediaId")]
        public virtual Media Media { get; set; }

        public virtual Institution Institution { get; set; }
        [NotMapped]
        public IFormFile fileImage { get; set; }
    }
}