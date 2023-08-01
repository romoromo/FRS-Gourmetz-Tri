using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class PIBTemplateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Description must be between 2 and 500 characters")]
        public string Description { get; set; }

        public string TemplateBody { get; set; }
        public string ImgUrl { get; set; }
        public string DeviceAPIUrl { get; set; }
        public string DeviceImageAPIUrl { get; set; }
        public bool IsPostToDevice { get; set; }
        public bool IsMapToAPI { get; set; }
        public string MapAPIUrl { get; set; }
        public bool IsPostImage { get; set; }

        public bool IsStaticLink { get; set; }
        public string MacAddress { get; set; }
        public string EpaperUrl { get; set; }

        public List<PIBTemplateLocationViewModel> Locations { get; set; }
        public List<long> LocationIds { get; set; }
    }
}
