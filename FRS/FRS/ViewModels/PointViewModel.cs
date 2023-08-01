using DAL.Models;
using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class PointViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }

        public int x { get; set; }
        public int y { get; set; }

        public bool IsFloorConnector { get; set; }

        public bool NoWheelchair { get; set; }

        public bool NoSheltered { get; set; }

        public int? MapId { get; set; }

        public bool IsActive { get; set; }
        public List<DirectoryListingViewModel> Directorys { get; set; }
        public List<LineViewModel> connectors { get; set; }
        public MapViewModel MapInfo { get; set; }
    }
}
