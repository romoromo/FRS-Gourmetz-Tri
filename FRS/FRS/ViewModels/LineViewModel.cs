using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class LineViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }

        public int? MapId { get; set; }

        public int? Point0Id { get; set; }

        public int? Point1Id { get; set; }

        public string Code0 { get; set; }
        public string Code1 { get; set; }

        public bool IsActive { get; set; }
        public string Label0 { get; set; }
        public string Label1 { get; set; }

        public string DisplayLabel { get; set; }

        public bool isFloorConnector { get; set; }
    }
}
