using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class DirectoryListingViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }


        public int? DirectoryListingCategoryId { get; set; }
        public string DirectoryListingCategoryCode { get; set; }
        public string DirectoryListingCategoryLabel { get; set; }

        public int? FloorId { get; set; }
        public string FloorLabel { get; set; }






        public string UnitNumber { get; set; }


        public string Aliases { get; set; }



        public string Contact { get; set; }






        public string MapDisplayName { get; set; }


        public bool AvailableForBooking { get; set; }

        public bool IsInternal { get; set; }



        

        public string IconUrl { get; set; }

        public string ExtraField { get; set; }

        public string Disciplines { get; set; }

        public string HighlightIcon { get; set; }
        public string HighlightText { get; set; }
        public bool Pause { get; set; }
        public int? PauseTime { get; set; }

        public int? PointId { get; set; }
        public int? DirectoryListingId { get; set; }
        public bool isActive { get; set; }
    }
}
