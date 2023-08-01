using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class EpaperTemplateLocationViewModel
    {
        public int Id { get; set; }
        public long LocationId { get; set; }
        public string Name { get; set; }

        public int? EpaperTemplateId { get; set; }
        public EpaperTemplateViewModel EpaperTemplate { get; set; }

        public LocationViewModel Location { get; set; }

    }
}
