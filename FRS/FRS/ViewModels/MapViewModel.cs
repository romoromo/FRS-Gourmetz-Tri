using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class MapViewModel
    {
        public int width { get; set; }
        public int height { get; set; }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }

        public int? FloorId { get; set; }

        public string map_url { get; set; }

        public List<PointViewModel> Points { get; set; }
        public List<LineViewModel> Lines { get; set; }
        public string FloorLabel { get; set; }
        public int? FloorOrder { get; internal set; }
    }
}
