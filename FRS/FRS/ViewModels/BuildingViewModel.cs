using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class BuildingViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }

        public List<FloorViewModel> floors { get; set; }
    }
}
