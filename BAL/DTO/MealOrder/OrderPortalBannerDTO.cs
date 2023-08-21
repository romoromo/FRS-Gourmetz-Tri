using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class OrderPortalBannerDTO
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public int Order { get; set; }

        public int? ImageId { get; set; }

        public int? OrderPortalContentId { get; set; }

        public string ImageFileName { get; set; }

        public string ImageFilePath { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

    }
}
