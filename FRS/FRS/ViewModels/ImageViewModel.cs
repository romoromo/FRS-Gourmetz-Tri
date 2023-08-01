using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class ImageViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Image file is required")]
        public string ImageLocation { get; set; }

        public int InstitutionId { get; set; }

        public string Description { get; set; }
        public string Tags { get; set; }

        public string InstitutionName { get; set; }

        public bool IsVideo { get; set; }

        public int? MediaId { get; set; }

        public int imageIndex { get; set; }

        public int? duration { get; set; }
        public string animation { get; set; }
    }
}
