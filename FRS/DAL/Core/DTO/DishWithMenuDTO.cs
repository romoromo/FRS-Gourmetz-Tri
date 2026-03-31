using System;
using System.Collections.Generic;

namespace DAL.Core.DTO
{
    public class DishWithMenuDTO
    {
        public int MenuID { get; set; }
        public string Label { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<DishWithMenuDetailDTO> Dishes { get; set; }
    }

    public class DishWithMenuDetailDTO
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }
        public string SerialNumber { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
