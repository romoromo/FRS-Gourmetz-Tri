using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class MediaExtensionViewModel
    {
        public int Id { get; set; }
        public string Extension { get; set; }
        public string Label { get; set; }
        public int? min { get; set; }
        public int? max { get; set; }
    }
}
