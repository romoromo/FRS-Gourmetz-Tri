using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class PIBTemplateLocationViewModel
    {
        public int Id { get; set; }
        public long location_id { get; set; }
        public long hospital_id { get; set; }
        public string code { get; set; }
        public string label { get; set; }
        public string alias { get; set; }
        public int sequence { get; set; }
        public bool ward { get; set; }
        public bool bed { get; set; }

        public string ancestor_id { get; set; }
        public string ancestor_code { get; set; }
        public string ancestor_label { get; set; }
        public string ancestor_alias { get; set; }

        public int? PIBTemplateId { get; set; }
        public PIBTemplateViewModel PIBTemplate { get; set; }

        public PIBTemplateLocationViewModel ancestor { get; set; }

    }
}
