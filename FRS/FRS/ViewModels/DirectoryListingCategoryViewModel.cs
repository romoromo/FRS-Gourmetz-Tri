using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class DirectoryListingCategoryViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }
        public string Icon { get; set; }
    }
}
