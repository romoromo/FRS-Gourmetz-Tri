using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class ModuleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
        public string Name { get; set; }
        public string Route { get; set; }
        public string Uri { get; set; }

        public List<ModuleParameterViewModel> ModuleParameters { get; set; }
    }

    public class ModuleParameterViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int ModuleId { get; set; }
    }
}
