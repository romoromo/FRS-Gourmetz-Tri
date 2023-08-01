using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class InstitutionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Description must be between 2 and 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Time Start is required")]
        [Range(1, 24)]
        public int StartTime { get; set; }

        [Required(ErrorMessage = "Time End is required")]
        [Range(1, 24)]
        public int EndTime { get; set; }

        public bool IsRestrictDuplicateBooking { get; set; }

        public string Restriction { get; set; }

        public bool IsDefault { get; set; }
    }
}
