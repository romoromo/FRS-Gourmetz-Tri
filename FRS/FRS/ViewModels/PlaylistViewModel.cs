using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class PlaylistViewModel
    {
        public PlaylistViewModel()
        {
            ImageIds = new List<imageIndex>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
        public string Name { get; set; }

        public string Description { get; set; }

        public int InstitutionId { get; set; }

        public string InstitutionName { get; set; }

        public string ImageTitles { get; set; }
        public List<imageIndex> ImageIds { get; set; }

        public List<ImageViewModel> Images { get; set; }
    }

    public class imageIndex
    {
        public int? duration { get; set; }
        public string animation { get; set; }

        public imageIndex(int imageId, int imgIndex, int? duration, string animation)
        {
            this.imageId = imageId;
            this.imgIndex = imgIndex;
            this.duration = duration;
            this.animation = animation;
        }

        public int imageId { get; set; }

        public int imgIndex { get; set; }
    }
}
