using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class PIBTemplateLocation : AuditableEntity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public long location_id { get; set; }
        public long hospital_id { get; set; }
        public string code { get; set; }
        public string label { get; set; }
        public string alias { get; set; }
        public int sequence { get; set; }
        public bool ward { get; set; }
        public bool bed { get; set; }

        public long? ancestor_id { get; set; }
        public string ancestor_code { get; set; }
        public string ancestor_label { get; set; }
        public string ancestor_alias { get; set; }

        //[ForeignKey("ancestor_id")]
        //public virtual PIBTemplateLocation ancestor { get; set; }

        [ForeignKey("PIBTemplate")]
        public int? PIBTemplateId { get; set; }
        public virtual PIBTemplate PIBTemplate { get; set; }
    }
}
