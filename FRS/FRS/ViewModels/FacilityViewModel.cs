using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class FacilityViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Facility name is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Facility name must be between 2 and 200 characters")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }

        public string FilePath { get; set; }

        public byte[] FileBytes { get; set; }

        public int InstitutionId { get; set; }

        public string InstitutionName { get; set; }

        public string InstitutionDescription { get; set; }
    }
}
