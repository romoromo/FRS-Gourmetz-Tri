using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class ApplicationSettingViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Key is required"), StringLength(200, MinimumLength = 1, ErrorMessage = "Key must be between 1 and 200 characters")]
        public string Key { get; set; }

        [Required(ErrorMessage = "Value is required"), StringLength(200, MinimumLength = 1, ErrorMessage = "Value must be between 1 and 200 characters")]
        public string Value { get; set; }

        [Required(ErrorMessage = "Description is required"), StringLength(200, MinimumLength = 1, ErrorMessage = "Description must be between 1 and 200 characters")]
        public string Description { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }
    }
}
