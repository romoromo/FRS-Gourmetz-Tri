using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class EmsProfile : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }

        public int ScreenStatus { get; set; }

        public string Rotation { get; set; }
        public string Resolution { get; set; }

        public int? Brightness { get; set; }
        public int? Volume { get; set; }

        public string module_path { get; set; }
        public string module_path_value { get; set; }

        public bool reboot { get; set; }

        //public int? CommunicatorMenuId { get; set; }

        //[ForeignKey("CommunicatorMenuId")]
        //public virtual CommunicatorMenu CommunicatorMenu { get; set; }
    }
}
