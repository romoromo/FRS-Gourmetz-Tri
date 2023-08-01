using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class MediaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Description must be between 2 and 500 characters")]
        public string Description { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }

        public long DirectorySize { get; set; }
        public long NumberOfFiles { get; set; }

        public string[] RolesArr { get; set; }

        public string[] UserGroupsArr { get; set; }
    }
}
