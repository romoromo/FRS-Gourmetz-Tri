using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class EmsProfileViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }

        public string Rotation { get; set; }
        public string Resolution { get; set; }
        public int? brightness { get; set; }
        public int? volume { get; set; }
        public int ScreenStatus { get; set; }

        public int? CommunicatorMenuId { get; set; }
        public string module_path { get; set; }
        public string module_path_value { get; set; }


        public bool reboot { get; set; }
    }
}
