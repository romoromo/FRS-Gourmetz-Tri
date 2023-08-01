using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{

    public class OrderCollectionDTO
    {
        public int OrderId { get; set; }
        public DateTime? TimeCollected { get; set; }
    }

    public class OrderReturnDTO
    {
        public int OrderId { get; set; }
        public string BentoCode { get; set; }
        public DateTime? TimeReturned { get; set; }
    }
}
