using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class SignageComponentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int? FixWidth { get; set; }
        public int? FixHeight { get; set; }
        public bool IsRatio { get; set; }

        public string BackgroundColor { get; set; }
        public string BackgroundImage { get; set; }

        public string ComponentType { get; set; }

        public string Configurations { get; set; }

        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int order { get; set; }

        public int CompilationId { get; set; }
        public int ComponentId { get; set; }

        public int? PreviewWidth { get; set; }
        public int? PreviewHeight { get; set; }

        public string CreatedByName { get; set; }
        public DateTime UpdatedDate { get; set; }

        public int IsActive { get; set; }
    }
}
