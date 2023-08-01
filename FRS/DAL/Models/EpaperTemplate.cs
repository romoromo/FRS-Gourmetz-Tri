using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class EpaperTemplate : AuditableEntity
    {
        public EpaperTemplate()
        {
            this.Locations = new HashSet<EpaperTemplateLocation>();
        }

        [Key]
        public int Id { get; set; }
        public string TemplateBody { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImgUrl { get; set; }
        public string DeviceAPIURl { get; set; }
        public string DeviceImageAPIUrl { get; set; }
        public bool IsPostToDevice { get; set; }
        public bool IsMapToAPI { get; set; }
        public string MapAPIUrl { get; set; }
        public string TemplateType { get; set; }
        public virtual ICollection<EpaperTemplateLocation> Locations { get; set; }
    }
}
